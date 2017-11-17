using System;
using System.Reflection;

namespace NightlyCode.Core.Configuration {

    /// <summary>
    /// allows to specify default values
    /// </summary>
    public class DefaultValueAttribute : Attribute {
        readonly object value;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public DefaultValueAttribute(object value) {
            this.value = value;
        }

        /// <summary>
        /// the specified value
        /// </summary>
        public object Value => value;

        /// <summary>
        /// get the default value attribute for the specified property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static DefaultValueAttribute Get(PropertyInfo property) {
            return (DefaultValueAttribute)GetCustomAttribute(property, typeof(DefaultValueAttribute));
        }
    }
}