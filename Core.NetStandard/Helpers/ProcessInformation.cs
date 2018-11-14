using System;
using System.Diagnostics;
using NightlyCode.Core.Logs;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// helper methods for process information
    /// </summary>
    public static class ProcessInformation {

        /// <summary>
        /// returns the process name of a process id
        /// </summary>
        /// <param name="pid">process id</param>
        /// <returns>name of the process, null if the process is not found or the process can't be accessed</returns>
        public static string GetProcessName(uint pid) {
            try {
                Process process = Process.GetProcessById((int)pid);
                return process.ProcessName;
            }
            catch(Exception e) {
                Logger.Warning(nameof(ProcessInformation), $"Unable to get process name of '{pid}'", e);
                return null;
            }
        }

    }
}