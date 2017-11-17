using System;
using System.Reflection;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// used to specify encoding of string data in a packet
    /// </summary>
    public class EncodingAttribute : Attribute {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="encoding"></param>
        public EncodingAttribute(StringEncoding encoding) {
            Encoding = encoding;
        }

        /// <summary>
        /// encoding
        /// </summary>
        public StringEncoding Encoding { get; }

        /// <summary>
        /// get the encoding of a property
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static EncodingAttribute Get(PropertyInfo info) {
            return GetCustomAttribute(info, typeof(EncodingAttribute)) as EncodingAttribute;
        }

        public static StringEncoding GetEncoding(PropertyInfo info) {
            EncodingAttribute attribute = Get(info);
            return attribute == null ? StringEncoding.ASCII : attribute.Encoding;
        }
    }
}