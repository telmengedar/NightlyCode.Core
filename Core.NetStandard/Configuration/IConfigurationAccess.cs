namespace NightlyCode.Core.Configuration {

    /// <summary>
    /// interface for configuration access
    /// </summary>
    public interface IConfigurationAccess {

        /// <summary>
        /// determines whether the configuration contains a value
        /// </summary>
        /// <param name="path">path to configuration value</param>
        /// <returns>true when configuration contains a value for the property, false otherwise</returns>
        bool ContainsValue(string path);

        /// <summary>
        /// get a config value
        /// </summary>
        /// <typeparam name="T">type of config value to get</typeparam>
        /// <param name="path">path to configuration value</param>
        /// <returns>configuration value</returns>
        T GetConfigValue<T>(string path);

        /// <summary>
        /// set a config value
        /// </summary>
        /// <typeparam name="T">type of config value</typeparam>
        /// <param name="path">path to configuration value</param>
        /// <param name="value">value to set</param>
        void SetConfigValue<T>(string path, T value);
    }
}