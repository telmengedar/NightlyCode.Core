using System;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// helper operations for arrays
    /// </summary>
    public static class ArrayOperations {
         
        /// <summary>
        /// determines whether the two array instances are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool AreEqual<T>(T[] lhs, T[] rhs) {
            if(lhs.Length != rhs.Length)
                return false;

            for(int i = 0; i < lhs.Length; ++i)
                if(!lhs[i].Equals(rhs[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// equal operation to be used as assert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void AreEqualAssert<T>(T[] lhs, T[] rhs) {
            if(lhs.Length != rhs.Length)
                throw new Exception("Array lengths do not match");

            for(int i = 0; i < lhs.Length; ++i)
                if(!lhs[i].Equals(rhs[i]))
                    throw new Exception(string.Format("Arrays differ at index {0}: <{1}> != <{2}>", i, lhs[i], rhs[i]));
        }
    }
}