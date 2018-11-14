using System.Diagnostics;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// measures how many times an action is executed per second
    /// </summary>
    public class ActionPerSecondComputer {
        float actions;
        float currentrate;

        float passed;

        /// <summary>
        /// call this when an action is executed to refresh rate
        /// </summary>
        /// <param name="time"></param>
        public void Measure(float time) {
            passed += time;
            ++actions;
            if(!(passed > 1.0f)) return;
            currentrate = actions / passed;
            actions = 0.0f;
            passed = 0.0f;
        }

        /// <summary>
        /// current rate
        /// </summary>
        public float CurrentRate {
            get { return currentrate; }
        }
    }

    /// <summary>
    /// <see cref="ActionPerSecondComputer"/> with integrated stopwatch
    /// </summary>
    public class ActionPerSecondWatcher : ActionPerSecondComputer {
        readonly Stopwatch stopwatch=new Stopwatch();

        public void Measure() {
            float elapsed = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            Measure(elapsed);
        }
    }
}