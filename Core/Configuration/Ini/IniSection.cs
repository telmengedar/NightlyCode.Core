using System.Collections.Generic;

namespace NightlyCode.Core.Configuration.Ini {

    /// <summary>
    /// section of an ini-file
    /// </summary>
    public class IniSection {
        readonly Dictionary<string, string> entries = new Dictionary<string, string>();

        /// <summary>
        /// access to the entries
        /// </summary>
        /// <param name="key">key of ini value</param>
        /// <returns>ini value</returns>
        public string this[string key] {
            get {
                string value;
                entries.TryGetValue(key, out value);
                return value;
            }
            set { entries[key] = value; }
        }

        /// <summary>
        /// all entries of the section
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Entries => entries;

        /// <summary>
        /// determines whether the section contains a key
        /// </summary>
        /// <param name="key">key of ini value</param>
        /// <returns>true when ini section contains key, false otherwise</returns>
        public bool Contains(string key) {
            return entries.ContainsKey(key);
        }
    }
}