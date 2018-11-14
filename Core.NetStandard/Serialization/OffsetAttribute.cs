using System;
using System.Reflection;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// attribute used to specify an offset of packet data
    /// </summary>
    public class OffsetAttribute : Attribute {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="offset"></param>
        public OffsetAttribute(int offset) {
            Offset = offset;
        }

        /// <summary>
        /// offset
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// get the offset of an property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static OffsetAttribute Get(PropertyInfo property) {
            return GetCustomAttribute(property, typeof(OffsetAttribute)) as OffsetAttribute;
        }
    }
}