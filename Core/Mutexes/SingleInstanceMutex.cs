using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace NightlyCode.Core.Mutexes {

    /// <summary>
    /// mutex which can be used to determine whether other instances of the application are running
    /// </summary>
    /// <remarks>
    /// modified code from <see cref="http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567"/> is used
    /// </remarks>
    public class SingleInstanceMutex : IDisposable {
        readonly string mutexId;
        bool hasHandle;
        Mutex mutex;

        /// <summary>
        /// creates a new <see cref="SingleInstanceMutex"/>
        /// </summary>
        public SingleInstanceMutex() {
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;

            // unique id for global mutex - Global prefix means it is global to the machine
            mutexId = $"Global\\{{{appGuid}}}";
        }

        /// <summary>
        /// tries to aquire a new mutex
        /// </summary>
        /// <returns></returns>
        public bool Aquire() {
            // Need a place to store a return value in Mutex() constructor call
            bool createdNew;

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            MutexAccessRule allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            MutexSecurity securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);
            try {
                // note, you may want to time out here instead of waiting forever
                // edited by acidzombie24
                // mutex.WaitOne(Timeout.Infinite, false);
                hasHandle = mutex.WaitOne(0, false);
            }
            catch(AbandonedMutexException) {
                // Log the fact that the mutex was abandoned in another process, it will still get acquired
                hasHandle = true;
                
            }
            return hasHandle;
        }

        /// <summary>
        /// releases the mutex
        /// </summary>
        public void Release() {
            // edited by acidzombie24, added if statement
            if (hasHandle)
                mutex.ReleaseMutex();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            Release();
        }
    }
}