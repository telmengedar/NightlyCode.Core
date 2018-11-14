using System;
using System.IO;
using NightlyCode.Core.Threading;

namespace NightlyCode.Core.Logs {

    /// <summary>
    /// cleans up old log files
    /// </summary>
    /// <remarks>
    /// This checks the log target every other day and cleans up every file which is older than the specified age.
    /// </remarks>
    public class LogFileCleaner : PeriodicWorker {
        readonly TimeSpan agethreshold = TimeSpan.FromDays(7);
        readonly TimeSpan checkthreshold = TimeSpan.FromDays(1.0);
        readonly string logtarget;
        string filefilter = "*.log";

        // set initial check to yesterday to start check immediately
        DateTime lastcheck = (DateTime.Now - TimeSpan.FromDays(1.0)).Date;

        /// <summary>
        /// creates a new <see cref="LogFileCleaner"/>
        /// </summary>
        /// <param name="logtarget">target directory which is checked for logfiles</param>
        public LogFileCleaner(string logtarget) {
            this.logtarget = logtarget;
        }

        /// <summary>
        /// creates a new <see cref="LogFileCleaner"/>
        /// </summary>
        /// <param name="logtarget">target directory which is checked for logfiles</param>
        /// <param name="agethreshold">maximum age a log file can reach before it is deleted</param>
        public LogFileCleaner(string logtarget, TimeSpan agethreshold)
            : this(logtarget) {
            this.agethreshold = agethreshold;
        }

        /// <summary>
        /// work which is to be executed periodically
        /// </summary>
        protected override void Work() {
            DateTime now = DateTime.Now;
            if(now - lastcheck < checkthreshold)
                return;

            try {
                CheckLogDirectory(now);
            }
            catch(Exception e) {
                Logger.Error(this, "Failed to check log directory", e);
            }
            lastcheck = now;
        }

        void CheckLogDirectory(DateTime now) {
            foreach(string file in Directory.GetFiles(logtarget, filefilter)) {
                DateTime lastwritetime;
                try {
                    lastwritetime = new FileInfo(file).LastWriteTime;
                }
                catch(Exception e) {
                    Logger.Warning(this, $"Unable to get last write time of '{file}'. Skipping check of file.", e);
                    continue;
                }

                if(now - lastwritetime < agethreshold)
                    continue;

                Logger.Info(this, $"Removing '{file}' as it has reached the maximum age for log files");
                try {
                    File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                    File.Delete(file);
                }
                catch(Exception e) {
                    Logger.Error(this, $"Failed to remove '{file}'", e);
                }
            }
        }
    }
}