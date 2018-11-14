using System;
using System.Collections.Generic;
using NightlyCode.Core.Threading;

namespace NightlyCode.Core.Collections.Cache
{

    /// <summary>
    /// cache where the entries decay after a time
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public class TimedCache<TKey, TObject> : Cache<TKey, TObject> {
        readonly List<TKey> decayed=new List<TKey>();

        readonly PeriodicTimer timer = new PeriodicTimer();
        readonly Dictionary<TKey, CacheEntry<TObject>> lookup = new Dictionary<TKey, CacheEntry<TObject>>();
    
        readonly Func<TKey, TObject> createobject;
        readonly double resolution;
        readonly bool disposable;

        readonly object accesslock = new object();

        /// <summary>
        /// creates a new <see cref="TimedCache{TKey,TObject}"/>
        /// </summary>
        /// <param name="createobject">func called on cache miss</param>
        /// <param name="resolution">resolution of decay timer</param>
        public TimedCache(Func<TKey, TObject> createobject, double resolution=1.0)
        {
            this.createobject = createobject;
            this.resolution = resolution;
            timer.Elapsed += OnTimerElapsed;
            disposable = typeof(IDisposable).IsAssignableFrom(typeof(TObject));
            timer.Start(TimeSpan.FromSeconds(resolution));
        }

        /// <summary>
        /// triggered when an entry in cache was refreshed
        /// </summary>
        public event Action<TKey, TObject> CacheEntryRefreshed;

        void OnTimerElapsed() {
            lock (accesslock)
            {
                decayed.Clear();
                foreach (KeyValuePair<TKey, CacheEntry<TObject>> entry in lookup)
                {
                    entry.Value.Decay -= resolution;
                    if (entry.Value.Decay <= 0.0) {
                        decayed.Add(entry.Key);
                    }
                }

                foreach(TKey key in decayed)
                    RemoveEntry(key, lookup[key].Value);
            }
        }

        /// <summary>
        /// access to cache
        /// </summary>
        /// <param name="key">key identifying item</param>
        /// <returns>object from cache</returns>
        public override TObject this[TKey key] => Get(key);

        /// <summary>
        /// determines whether the cache contains a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(TKey key) {
            lock(accesslock)
                return lookup.ContainsKey(key);
        }

        void RemoveEntry(TKey key, TObject value) {
            lock(accesslock) {
                if(disposable)
                    ((IDisposable)value).Dispose();
                lookup.Remove(key);
            }
            OnValueRemoved(key, value);
        }

        /// <summary>
        /// removes a value from the cache
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key) {
            if(!Contains(key))
                return;

            CacheEntry<TObject> entry;
            lock(accesslock)
                entry = lookup[key];
            RemoveEntry(key, entry.Value);
        }

        /// <summary>
        /// get a value from the cache
        /// </summary>
        /// <param name="key">key of value to get</param>
        /// <returns>entry in cache</returns>
        public TObject Get(TKey key)
        {
            lock(accesslock) {
                CacheEntry<TObject> entry;
                if(!lookup.TryGetValue(key, out entry))
                    lookup[key] = entry = new CacheEntry<TObject>(createobject(key));
                else entry.Decay = 360.0;

                return entry.Value;
            }
        }

        /// <summary>
        /// adds an entry to the cache
        /// </summary>
        /// <param name="key">key of entry to add</param>
        /// <param name="value">value to add</param>
        public void AddEntry(TKey key, TObject value)
        {
            lock(accesslock) {
                lookup[key] = new CacheEntry<TObject>(createobject(key));
            }
        }

    }
}
