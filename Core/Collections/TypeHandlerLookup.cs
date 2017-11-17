using System;
using System.Collections.Generic;

#if WINDOWS_UWP
using System.Reflection;
#endif

namespace NightlyCode.Core.Collections {

    /// <summary>
    /// used to hold handlers for types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeHandlerLookup<T> {
        readonly Dictionary<Type, T> lookup = new Dictionary<Type, T>();

        /// <summary>
        /// indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[Type key] {
            get { return GetHandler(key); }
            set { SetHandler(key, value); }
        }

        /// <summary>
        /// sets a handler for the specified key
        /// </summary>
        /// <param name="key">type to be handled</param>
        /// <param name="value">handler for the key</param>
        public void SetHandler(Type key, T value) {
            lookup[key] = value;
        }

        void PrepareHandler(Type key) {
            List<Type> typestosearch = new List<Type>();
#if WINDOWS_UWP
            Type current = key.GetTypeInfo().BaseType;
#else
            Type current = key.BaseType;
#endif
            while (current != null && current != typeof(object)) {
                typestosearch.Add(current);
#if WINDOWS_UWP
                current = current.GetTypeInfo().BaseType;
#else
                current = current.BaseType;
#endif
            }
            typestosearch.Add(typeof(object));
            typestosearch.AddRange(key.GetInterfaces());

            T result;
            foreach(Type type in typestosearch)
                if(lookup.TryGetValue(type, out result)) {
                    lookup[key] = result;

                    // handler was found. break here.
                    break;
                }

            // dont throw else containskey would need to catch and that is a bit expensive for a simple check
            //throw new InvalidOperationException(string.Format("No handler exists for '{0}'", key));
        }

        /// <summary>
        /// Determines whether the lookup contains a handler for the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Type key) {
            if(key == null)
                throw new ArgumentNullException(nameof(key));

            if(lookup.ContainsKey(key))
                return true;

            PrepareHandler(key);

            return lookup.ContainsKey(key);
        }

        /// <summary>
        /// get the first handler which can handle the specified type
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetHandler(Type key) {
            if(key==null)
                throw new ArgumentNullException(nameof(key));

            T result;

            if(lookup.TryGetValue(key, out result))
                return result;

            PrepareHandler(key);
            return lookup[key];
        }
    }
}