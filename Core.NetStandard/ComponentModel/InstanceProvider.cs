using System;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// provides a single instance of something by threadsafe creation on first access
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceProvider<T> 
        where T : class 
    {
        T instance;
        readonly Func<T> instancecreator;
        readonly object locker = new object();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="instancecreator"></param>
        public InstanceProvider(Func<T> instancecreator) {
            this.instancecreator = instancecreator;
        }

        /// <summary>
        /// determines whether an instance was created
        /// </summary>
        public bool InstanceCreated { get; private set; }

        /// <summary>
        /// invalidates the instance
        /// </summary>
        public void Invalidate() {
            lock(locker) {
                if(instance is IDisposable)
                    ((IDisposable)instance).Dispose();
                instance = null;
                InstanceCreated = false;
            }
        }

        T CreateInstance() {
            InstanceCreated = true;
            return instancecreator();
        }
        /// <summary>
        /// instance
        /// </summary>
        public T Instance {
            get {
                lock(locker) {
                    return instance ?? (instance = CreateInstance());
                }
            }
        }
    }
}