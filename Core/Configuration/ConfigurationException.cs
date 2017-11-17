using System;
using System.Runtime.Serialization;

namespace NightlyCode.Core.Configuration {

    /// <summary>
    /// exception when accessing configuration
    /// </summary>
    public class ConfigurationException : Exception {

        /// <summary>
        /// creates a new <see cref="ConfigurationException"/>
        /// </summary>
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}

        /// <summary>
        /// creates a new <see cref="ConfigurationException"/>
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="innerException">exception which led to this exception</param>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// creates a new <see cref="ConfigurationException"/>
        /// </summary>
        /// <param name="message">exception message</param>
        public ConfigurationException(string message)
            : base(message) {}
    }
}