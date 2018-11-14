using System;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Core.Randoms;

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// provides operations for enumerations
    /// </summary>
    public static class EnumerationExtensions {

        /// <summary>
        /// get the index of a matching item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, Func<T, bool> func) {
            int i = 0;
            foreach(T item in enumeration) {
                if(func(item)) return i;
                ++i;
            }
            return -1;
        }

        /// <summary>
        /// converts all items in the enumeration using the specified func
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> Convert<TIn, TOut>(this IEnumerable<TIn> enumeration, Func<TIn, TOut> func) {
            foreach(TIn item in enumeration)
                yield return func(item);
        }

        /// <summary>
        /// basically an alias for IEnumerable.Any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> enumeration, Predicate<T> predicate) {
            foreach (T item in enumeration)
                if(predicate(item))
                    return true;
            return false;
        }

        /// <summary>
        /// get intersection of two enumerations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs) {
            HashSet<T> lhsitems = new HashSet<T>(lhs);
            foreach(T item in rhs)
                if(lhsitems.Contains(item))
                    yield return item;
        }
 
        /// <summary>
        /// determines whether the enumerations are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool AreEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs) {
            T[] rhsarray = rhs.ToArray();
            foreach(T item in lhs) {
                T comparision = item;
                if(!rhsarray.Contains(obj => obj.Equals(comparision)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// get the hashcode of the enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <returns></returns>
        public static int Hashcode<T>(this IEnumerable<T> lhs) {
            int hashcode = 0;
            foreach(T item in lhs)
                if(item != null)
                    hashcode ^= item.GetHashCode();
            return hashcode;
        }

        /// <summary>
        /// executes an action foreach item in the enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="action"> </param>
        public static void Foreach<T>(this IEnumerable<T> lhs, Action<T> action) {
            foreach(T item in lhs)
                action(item);
        }

        /// <summary>
        /// removes insignificant values from an enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="evaluator"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveInsignificant<T>(this IEnumerable<T> enumeration, Func<T, double> evaluator, double threshold) {
            double median = enumeration.Sum(evaluator) / enumeration.Count();
            foreach(T item in enumeration)
                if(evaluator(item) >= median * threshold)
                    yield return item;
        }

        /// <summary>
        /// normalizes values of items for statistical purposes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="evaluator"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static IEnumerable<T> NormalizeValues<T>(this IEnumerable<T> enumeration, Func<T, double> evaluator, Func<T, double, T> modifier) {
            double sum = enumeration.Sum(evaluator);
            foreach(T item in enumeration)
                yield return modifier(item, evaluator(item) / sum);
        }

        /// <summary>
        /// splits the enumeration to blocks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration">enumeration to be split</param>
        /// <param name="blocksize">size of blocks</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Block<T>(this IEnumerable<T> enumeration, int blocksize) {
            int currentblock = blocksize;
            List<T> items = new List<T>();
            foreach(T item in enumeration) {
                items.Add(item);
                if(--currentblock == 0) {
                    yield return items.ToArray();
                    currentblock = blocksize;
                    items.Clear();
                }
            }
            if(currentblock != blocksize)
                yield return items.ToArray();
        }

        /// <summary>
        /// creates a sequence
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<int> Sequence(int start, int end, int step=1) {
            for(int i = start; i < end; i += step)
                yield return i;
        }

        /// <summary>
        /// creates an enumeration using the specified creator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static IEnumerable<T> Create<T>(int count, Func<int, T> creator) {
            for(int i = 0; i < count; ++i)
                yield return creator(i);
        }

        /// <summary>
        /// shuffles the enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, RNG rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                int swapIndex = rng.NextInt(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

    }
}