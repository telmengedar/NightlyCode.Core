using System;
using System.Reflection;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// used to specify endianess of data
    /// </summary>
    public class EndianessAttribute : Attribute {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="endianess"></param>
        public EndianessAttribute(Endianess endianess) {
            Endianess = endianess;
        }

        /// <summary>
        /// endianess
        /// </summary>
        public Endianess Endianess { get; set; }

        /// <summary>
        /// get the attribute of the specified property
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static EndianessAttribute Get(PropertyInfo info) {
            return (EndianessAttribute)GetCustomAttribute(info, typeof(EndianessAttribute));
        }
    }
}