using System;

namespace NightlyCode.Core.Collections.Cache {

    /// <summary>
    /// base class for caches
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class Cache<TKey, TValue> : ICache<TKey, TValue> {

        /// <summary>
        /// access to cache
        /// </summary>
        /// <param name="key">key identifying item</param>
        /// <returns>object from cache</returns>
        public abstract TValue this[TKey key] { get; }

        /// <summary>
        /// triggered when a value is removed from the cache
        /// </summary>
        public event Action<ICache<TKey, TValue>, TKey, TValue> ValueRemoved;

        /// <summary>
        /// to be called when a value was removed
        /// </summary>
        /// <param name="key">key of removed value</param>
        /// <param name="value">removed value</param>
        protected void OnValueRemoved(TKey key, TValue value) {
            ValueRemoved?.Invoke(this, key, value);
        }
    }
}