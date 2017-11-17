using System;
using System.Collections.Generic;
using NightlyCode.Core.Threading;

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// collection which removes items after a specific time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DecayingCollection<T> : PeriodicWorker
        where T : IDecayingItem {
        readonly List<T> items = new List<T>();
        readonly object itemlock = new object();

        /// <summary>
        /// creates a new <see cref="DecayingCollection{T}"/>
        /// </summary>
        public DecayingCollection()
            : this(TimeSpan.FromMinutes(5.0))
        { }

        /// <summary>
        /// creates a new <see cref="DecayingCollection{T}"/>
        /// </summary>
        /// <param name="decaythreshold"></param>
        public DecayingCollection(TimeSpan decaythreshold) {
            DecayThreshold = decaythreshold;
        }

        /// <summary>
        /// minimum age of items which are to be removed from the collection
        /// </summary>
        public TimeSpan DecayThreshold { get; }

        /// <summary>
        /// items in collection
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                lock(itemlock)
                    foreach(T item in items)
                        yield return item;
            }
        }

        /// <summary>
        /// adds an item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            lock(itemlock)
                items.Add(item);
        }

        /// <summary>
        /// work which is to be executed periodically
        /// </summary>
        protected override void Work() {
            lock(itemlock) {
                DateTime now = DateTime.Now;
                for(int i=items.Count-1;i>=0;--i)
                    if(now - items[i].LastRefresh >= DecayThreshold)
                        items.RemoveAt(i);
            }
        }
    }
}