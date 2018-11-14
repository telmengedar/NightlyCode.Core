using System;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// specifies a method to use to parse data from a string
    /// </summary>
    public class ParserAttribute : Attribute {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="method"></param>
        public ParserAttribute(string method) {
            Method = method;
        }

        /// <summary>
        /// name of method to use
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// determines whether the attribute is defined for the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefined(Type type) {
            return IsDefined(type, typeof(ParserAttribute));
        }

        /// <summary>
        /// get attribute for a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ParserAttribute Get(Type type) {
            return GetCustomAttribute(type, typeof(ParserAttribute)) as ParserAttribute;
        }
    }
}