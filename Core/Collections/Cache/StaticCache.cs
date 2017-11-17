using System;
using System.Collections.Generic;

namespace NightlyCode.Core.Collections.Cache {

    /// <summary>
    /// a cache which values never decay (which actually means a wrapper for a dictionary)
    /// </summary>
    /// <typeparam name="TKey">type of key</typeparam>
    /// <typeparam name="TValue">type of value</typeparam>
    public class StaticCache<TKey, TValue> : Cache<TKey, TValue> {
        readonly object cachelock = new object();
        readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

        readonly Func<TKey, TValue> creator;

        /// <summary>
        /// creates a new <see cref="StaticCache{TKey,TValue}"/>
        /// </summary>
        /// <param name="creator">creator for values using a key</param>
        public StaticCache(Func<TKey, TValue> creator) {
            this.creator = creator;
        }

        /// <summary>
        /// get a value from the cache
        /// </summary>
        /// <param name="key">key for value to get</param>
        /// <returns>value corresponding to key</returns>
        public TValue GetValue(TKey key) {
            TValue value;
            lock(cachelock)
                if(!cache.TryGetValue(key, out value))
                    cache[key] = value = creator(key);
            return value;
        }

        /// <summary>
        /// access to cache
        /// </summary>
        /// <param name="key">key identifying item</param>
        /// <returns>object from cache</returns>
        public override TValue this[TKey key] => GetValue(key);
    }
}