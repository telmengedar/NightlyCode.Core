using System.Collections.Generic;

namespace NightlyCode.Core {

    /// <summary>
    /// wrapper for commandline arguments
    /// </summary>
    public class ArgumentCollection {
        readonly Dictionary<string, string> arguments = new Dictionary<string, string>();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="args"></param>
        public ArgumentCollection(IEnumerable<string> args) {
            foreach(string arg in args) {
                string[] split = arg.Split('=');
                if(split.Length == 2)
                    arguments[split[0]] = split[1];
            }
        }

        /// <summary>
        /// indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] => GetValue(key);

        /// <summary>
        /// determines whether the collection contains a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key) {
            return arguments.ContainsKey(key);
        }

        /// <summary>
        /// get the value of the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key) {
            return arguments[key];
        }
    }
}