using System;
using System.Threading;
using System.Threading.Tasks;

namespace NightlyCode.Core.Threading {

    /// <summary>
    /// base class which starts work periodically
    /// </summary>
    public abstract class PeriodicWorker : IDisposable {
        readonly TimeSpan noperiod = new TimeSpan(0, 0, 0, 0, -1);
        TimeSpan interval;
        bool enabled;
        Timer timer;

        /// <summary>
        /// starts the timer
        /// </summary>
        public void Start(TimeSpan workinterval) {
            interval = workinterval;
            if(enabled)
                return;
            enabled = true;
            OnStart();

            // start timer in background because it blocks main thread when due time is zero
            new Task(StartTimer).Start();
        }

        void StartTimer() {
            timer = new Timer(state => TriggerWork(), null, TimeSpan.Zero, noperiod);
        }

        /// <summary>
        /// triggered when a step was processed
        /// </summary>
        public event Action WorkStepDone;

        void OnWorkStepDone() {
            WorkStepDone?.Invoke();
        }

        /// <summary>
        /// method called when timer is stopped
        /// </summary>
        protected virtual void OnStop() {
        }

        /// <summary>
        /// method called when timer is started
        /// </summary>
        protected virtual void OnStart() {

        }

        /// <summary>
        /// stops the timer
        /// </summary>
        public void Stop() {
            if(!enabled)
                return;
            enabled = false;
            OnStop();
        }

        /// <summary>
        /// triggers the execution of the work method manually
        /// </summary>
        public void TriggerWork() {
            try {
                Work();
                OnWorkStepDone();
            }
            catch(Exception e) {
                OnErrorOccured(e);
            }
            finally {
                // has to be called in any case ... not just successful work
                // else the worker stops working and no one knows why
                if(enabled)
                    timer.Change(interval, noperiod);
            }
        }

        /// <summary>
        /// work which is to be executed periodically
        /// </summary>
        protected abstract void Work();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            Stop();
        }

        /// <summary>
        /// triggered when an error occured during execution
        /// </summary>
        public event Action<Exception> ErrorOccured;

        /// <summary>
        /// method called when an error has occured while executing timer function
        /// </summary>
        /// <param name="error">error which occured</param>
        protected virtual void OnErrorOccured(Exception error) {
            ErrorOccured?.Invoke(error);
        }
    }
}