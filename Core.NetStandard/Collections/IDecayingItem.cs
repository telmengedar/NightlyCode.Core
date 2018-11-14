using System;

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// item for <see cref="DecayingCollection{T}"/>
    /// </summary>
    public interface IDecayingItem {

        /// <summary>
        /// last time item was refreshed
        /// </summary>
        DateTime LastRefresh { get; set; } 
    }
}