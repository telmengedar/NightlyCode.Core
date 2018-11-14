using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// pipeline of actions executed in parallel
    /// </summary>
    public class ActionPipeline<TItem> {
        readonly List<PipelineProcessor> processors = new List<PipelineProcessor>();
        readonly int buffersize;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="buffersize"></param>
        public ActionPipeline(int buffersize) {
            this.buffersize = buffersize;
        }

        /// <summary>
        /// adds an action to the pipeline
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="action"></param>
        public void AddAction<TIn, TOut>(Func<TIn, TOut> action) 
            where TIn : class
            where TOut : class {
            PipelineProcessor<TIn, TOut> processor = new PipelineProcessor<TIn, TOut>(buffersize, action);
            if(processors.Count>0)
                processors[processors.Count - 1].NextProcessor = processor;
            processors.Add(processor);
        }

        /// <summary>
        /// executes the pipeline for the specified item
        /// </summary>
        /// <param name="item"></param>
        public void Execute(TItem item) {
            processors.First().AddItem(item);
        }
    }

    /// <summary>
    /// internal class for use in pipeline
    /// </summary>
    public abstract class PipelineProcessor {

        /// <summary>
        /// adds an item to the action
        /// </summary>
        /// <param name="item"></param>
        internal abstract void AddItem(object item);

        /// <summary>
        /// next processor in line
        /// </summary>
        internal PipelineProcessor NextProcessor { get; set; }
    }

    /// <summary>
    /// pipeline processor for a typed action
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class PipelineProcessor<TIn, TOut> : PipelineProcessor
        where TIn : class
        where TOut : class
    {
        readonly Queue<TIn> itemstoprocess=new Queue<TIn>();

        readonly object processingwait = new object();
        bool processingitem;

        readonly int buffersize;
        readonly Func<TIn, TOut> processor;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="buffersize"></param>
        /// <param name="processor"></param>
        public PipelineProcessor(int buffersize, Func<TIn, TOut> processor) {
            this.buffersize = buffersize;
            this.processor = processor;
            new Task(Processor, TaskCreationOptions.LongRunning).Start();
        }

        internal override void AddItem(object item) {

            lock(processingwait) {
                if(itemstoprocess.Count >= buffersize)
                    Monitor.Wait(processingwait);

                itemstoprocess.Enqueue((TIn)item);
                if(!processingitem)
                    Monitor.Pulse(processingwait);
            }
        }

        void Processor() {
            while(true) {
                TIn item;
                lock(processingwait) {
                    if(itemstoprocess.Count > 0) {
                        item = itemstoprocess.Dequeue();
                        Monitor.Pulse(processingwait);
                    }
                    else item = null;
                }

                if(item != null) {
                    lock(processingwait)
                        processingitem = true;

                    TOut result = processor(item);

                    if(result != null && NextProcessor != null)
                        NextProcessor.AddItem(result);
                }
                else {
                    lock(processingwait) {
                        processingitem = false;
                        Monitor.Wait(processingwait);
                    }
                }   
            }
        }
    }
}