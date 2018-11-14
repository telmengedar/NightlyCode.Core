using System;

namespace NightlyCode.Core.Collections.Cache {

    /// <summary>
    /// provides a cache for values
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface ICache<TKey, TValue> {

        /// <summary>
        /// access to cache
        /// </summary>
        /// <param name="key">key identifying item</param>
        /// <returns>object from cache</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// triggered when a value is removed from the cache
        /// </summary>
        event Action<ICache<TKey, TValue>, TKey, TValue> ValueRemoved;
    }
}