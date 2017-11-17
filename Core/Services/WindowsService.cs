using System;
using System.Diagnostics;
using System.ServiceProcess;
using NightlyCode.Core.Logs;

namespace NightlyCode.Core.Services {

    /// <summary>
    /// base class for services
    /// </summary>
    public abstract class WindowsService : ServiceBase {

        /// <summary>
        /// starts service
        /// </summary>
        public void StartService() {
            if(IsService()) {
                Run(this);
            }
            else {
                Logger.EnableConsoleLogging();
                OnStart(new string[0]);
                Console.ReadKey();
                OnStop();
            }
        }

        bool IsService()
        {
            Process p = ParentProcessUtilities.GetParentProcess();
            return p != null && p.ProcessName == "services";
        }

    }
}