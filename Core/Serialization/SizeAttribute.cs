using System;
using System.Reflection;

namespace NightlyCode.Core.Serialization {

    /// <summary>
    /// specified the size of data (eg. byte array)
    /// </summary>
    public class SizeAttribute : Attribute {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="size"></param>
        public SizeAttribute(int size) {
            Size = size;
        }

        /// <summary>
        /// size of data
        /// </summary>
        public int Size { get; private set; }
         
        /// <summary>
        /// get the size attribute for the specified member
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static SizeAttribute Get(MemberInfo member) {
            return GetCustomAttribute(member, typeof(SizeAttribute)) as SizeAttribute;
        }

        /// <summary>
        /// get size defined for member or 1 if attribute isn't defined
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static int GetSize(MemberInfo member) {
            SizeAttribute attribute = Get(member);
            return attribute != null ? attribute.Size : -1;
        }
    }
}