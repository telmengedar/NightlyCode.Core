using System;

namespace NightlyCode.Core.Configuration.Ini {

    /// <summary>
    /// access to an ini section
    /// </summary>
    public class IniSectionAccess : IConfigurationAccess {
        readonly IniFile inifile;
        readonly string section;

        /// <summary>
        /// creates a new <see cref="IniSectionAccess"/>
        /// </summary>
        /// <param name="inifile">ini file which contains section</param>
        /// <param name="section">name of section</param>
        public IniSectionAccess(IniFile inifile, string section) {
            this.inifile = inifile;
            this.section = section;
        }

        /// <summary>
        /// determines whether the configuration contains a value
        /// </summary>
        /// <param name="path">path to configuration value</param>
        /// <returns>true when configuration contains a value for the property, false otherwise</returns>
        public bool ContainsValue(string path) {
            return inifile.Contains(section) && inifile[section].Contains(path);
        }

        /// <summary>
        /// get a config value
        /// </summary>
        /// <typeparam name="T">type of config value to get</typeparam>
        /// <param name="path">path to configuration value</param>
        /// <returns>configuration value</returns>
        public T GetConfigValue<T>(string path) {
            return (T)Convert.ChangeType(inifile[section][path], typeof(T));
        }

        /// <summary>
        /// set a config value
        /// </summary>
        /// <typeparam name="T">type of config value</typeparam>
        /// <param name="path">path to configuration value</param>
        /// <param name="value">value to set</param>
        public void SetConfigValue<T>(string path, T value) {
            if(value == null) inifile[section][path] = "";
            else inifile[section][path] = value.ToString();
        }
    }
}