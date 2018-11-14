using System.Collections.Generic;
using System.Linq;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// arguments for programs
    /// </summary>
    public class Arguments {
        readonly Dictionary<string, string> arguments = new Dictionary<string, string>();

        /// <summary>
        /// accessor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return GetOrDefault(key); }
            private set { arguments[key] = value; }
        }

        /// <summary>
        /// determines whether arguments contain a specific key
        /// </summary>
        /// <param name="key">key to check for</param>
        /// <returns></returns>
        public bool Contains(string key) {
            return arguments.ContainsKey(key);
        }

        /// <summary>
        /// get value for a key or default if not found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public string GetOrDefault(string key, string defaultvalue=null) {
            string value;
            if(!arguments.TryGetValue(key, out value))
                return defaultvalue;
            return value;
        }

        /// <summary>
        /// parses arguments from args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Arguments Parse(params string[] args) {
            Arguments arguments = new Arguments();

            for (int i = 0; i < args.Length; ++i) {
                string key = args[i];
                arguments[key] = args[++i];
            }

            return arguments;
        }

        public override string ToString() {
            return string.Join(" ", arguments.Select(a => a.Key + " \"" + a.Value + "\""));
        }
    }
}