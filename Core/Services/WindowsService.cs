using System;
using System.Diagnostics;
using System.Linq;
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
                OnStart(Environment.GetCommandLineArgs().Skip(1).ToArray());
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