using System.Collections.Generic;
using System.IO;

namespace NightlyCode.Core.Configuration.Ini {

    /// <summary>
    /// file with the structure of a ini file
    /// </summary>
    public class IniFile {
        readonly Dictionary<string, IniSection> sections = new Dictionary<string, IniSection>();

        /// <summary>
        /// access to the entries
        /// </summary>
        /// <param name="key">key of ini section to get</param>
        /// <returns>ini section</returns>
        public IniSection this[string key] {
            get {
                IniSection value;
                if (!sections.TryGetValue(key, out value))
                    sections[key] = value = new IniSection();
                return value;
            }
            set { sections[key] = value; }
        }

        /// <summary>
        /// determines whether the section contains a key
        /// </summary>
        /// <param name="key">key of ini section</param>
        /// <returns>true when ini file contains section, false otherwise</returns>
        public bool Contains(string key) {
            return sections.ContainsKey(key);
        }

        /// <summary>
        /// loads an ini file
        /// </summary>
        /// <returns>loaded ini file</returns>
        public static IniFile Load(string path) {
            string[] lines = File.ReadAllLines(path);

            IniFile file = new IniFile();
            IniSection section = null;
            foreach(string line in lines) {
                if(string.IsNullOrEmpty(line)) continue;

                if(line[0] == '[') {
                    section = new IniSection();
                    file[line.Substring(1, line.Length - 2)] = section;
                }
                else {
                    if(section == null) continue;

                    int idx = line.IndexOf('=');
                    if(idx>=0) {
                        section[line.Substring(0, idx)] = line.Substring(idx + 1);
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// saves the ini to a stream
        /// </summary>
        /// <param name="stream">stream to which to save ini file</param>
        public void Save(Stream stream) {
            using(StreamWriter writer = new StreamWriter(stream)) {
                foreach(KeyValuePair<string, IniSection> section in sections) {
                    writer.WriteLine("[{0}]", section.Key);
                    foreach(KeyValuePair<string, string> entry in section.Value.Entries) {
                        writer.WriteLine("{0}={1}", entry.Key, entry.Value);
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}