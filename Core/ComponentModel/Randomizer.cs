using System;
using System.Collections.Generic;
using System.Linq;
using NightlyCode.Core.Randoms;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// operations using a randomizer
    /// </summary>
    public static class Randomizer {

        /// <summary>
        /// gets a random item out of an evaluated enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomItem<T>(this IEnumerable<T> items, Func<T, double> evaluator, RNG rng) {
            // i feel like this is biased towards items with higher values (more than it should be)
            // return items.OrderBy(i => evaluator(i) * rnd.NextDouble()).LastOrDefault();


            T[] enumerable = items as T[] ?? items.ToArray();
            if(enumerable.Length == 0)
                return default(T);

            double sum = enumerable.Sum(evaluator);
            double value = rng.NextDouble() * sum;
            double current = 0.0f;
            foreach(T item in enumerable) {
                current += evaluator(item);
                if(current >= value)
                    return item;
            }
            throw new Exception("Random value not in range of 0-1 (which is an error with the random number generator)");
        }

        /// <summary>
        /// returns random item of an enumeration
        /// </summary>
        /// <typeparam name="T">type of item in enumeration</typeparam>
        /// <param name="items">item enumeration</param>
        /// <param name="rng">rng to use</param>
        /// <returns>random item</returns>
        public static T RandomItem<T>(this IEnumerable<T> items, RNG rng) {
            T[] array = items as T[] ?? items.ToArray();
            if(array.Length == 0)
                return default(T);
            return array[rng.NextInt(array.Length)];
        }
    }
}