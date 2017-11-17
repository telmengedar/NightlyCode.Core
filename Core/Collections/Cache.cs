using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GM.Core.Threading;
using System.Diagnostics;
using GM.Core.Logs;
using GM.Core.ComponentModel;

namespace GM.Core.Collections
{
    public class Cache<TKey, TObject> : IJob
    {
        public delegate TObject CreateObjectDelegate(TKey key);

        Dictionary<TKey, TObject> lookup = new Dictionary<TKey, TObject>();
        ILock lookuplock = new MonitorLock();
        ILock lifetimelock = new MonitorLock();
        Dictionary<TKey, TimeSpan> lifetimes = new Dictionary<TKey, TimeSpan>();
        HashSet<TKey> keystorefresh = new HashSet<TKey>();
        TimeSpan lifetime = TimeSpan.FromMinutes(6.0);

        CreateObjectDelegate createobject;
        bool disposable;

        SecureEvent<TObject> objectremoved = new SecureEvent<TObject>();

        public Cache(CreateObjectDelegate createobject)
        {
            this.createobject = createobject;
            TimedJobManager.AddJob(this, TimeSpan.FromSeconds(1.0));
            disposable = typeof(IDisposable).IsAssignableFrom(typeof(TObject));
        }

        public TObject this[TKey key] {
            get { return Get(key); }
        }

        public event SecureEventHandler<TObject> ObjectRemoved {
            add { objectremoved.Event += value; }
            remove { objectremoved.Event -= value; }
        }

        void IJob.Execute()
        {
            TimeSpan elapsed = TimeSpan.FromSeconds(1.0);

            lifetimelock.Enter();
            foreach (TKey key in keystorefresh)
                lifetimes[key] = lifetime;
            keystorefresh.Clear();
            lifetimelock.Exit();

            List<TKey> keys=new List<TKey>(lifetimes.Keys);
            foreach (TKey key in keys)
            {
                lifetimes[key] -= elapsed;
                if (lifetimes[key].Ticks <= 0)
                {
                    LogManager.Log(new LogTextMessage("Cache", string.Format("removing {0} from cache", key)));

                    bool result;
                    TObject obj;
                    lookuplock.Enter();
                    if(result = lookup.TryGetValue(key, out obj)) {
                        if(disposable)
                            ((IDisposable)obj).Dispose();
                    }
                    lookup.Remove(key);

                    lifetimelock.Enter();
                    lifetimes.Remove(key);
                    lifetimelock.Exit();

                    lookuplock.Exit();

                    if(result)
                        objectremoved.Invoke(this, obj);
                }
            }
        }

        public void Remove(TKey key)
        {
            lifetimelock.Enter();
            keystorefresh.Add(key);
            lifetimelock.Exit();
        }

        public TObject Get(TKey key)
        {
            TObject obj;
            lookuplock.Enter();
            if (!lookup.TryGetValue(key, out obj))
                lookup[key] = obj = createobject(key);

            lifetimelock.Enter();
            keystorefresh.Add(key);
            lifetimelock.Exit();

            lookuplock.Exit();

            return obj;
        }
    }
}
