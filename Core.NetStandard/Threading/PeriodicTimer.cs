using System;

namespace NightlyCode.Core.Threading {

    /// <summary>
    /// timer which signals periodically
    /// </summary>
    public class PeriodicTimer : PeriodicWorker {

        /// <summary>
        /// triggered when timer has elapsed
        /// </summary>
        public event Action Elapsed;

        void OnElapsed() {
            Elapsed?.Invoke();
        }

        protected override void Work() {
            OnElapsed();
        }
    }
}