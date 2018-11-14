namespace NightlyCode.Core.Collections.Cache {

    /// <summary>
    /// entry for the timed cache
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class CacheEntry<TValue> {
        readonly TValue value;

        /// <summary>
        /// creates a new <see cref="CacheEntry{TValue}"/>
        /// </summary>
        /// <param name="value">value to be stored in cache</param>
        /// <param name="decay">time until value is removed from cache</param>
        public CacheEntry(TValue value, double decay=360.0) {
            this.value = value;
            Decay = decay;
        }

        /// <summary>
        /// value
        /// </summary>
        public TValue Value => value;

        /// <summary>
        /// time after which the value decays
        /// </summary>
        public double Decay { get; set; }
    }
}