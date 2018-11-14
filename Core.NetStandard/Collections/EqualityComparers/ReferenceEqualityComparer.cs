using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NightlyCode.Core.Collections.EqualityComparers {

    /// <summary>
    /// equality comparer for reference equality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T> {

        public bool Equals(T x, T y) {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj) {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}