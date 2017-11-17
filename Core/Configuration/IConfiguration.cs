using System.ComponentModel;

namespace NightlyCode.Core.Configuration {

    /// <summary>
    /// interface for a configuration
    /// </summary>
    public interface IConfiguration : INotifyPropertyChanged {

        /// <summary>
        /// reverts the property to its default value
        /// </summary>
        /// <param name="property">name of property to revert</param>
        void RevertToDefault(string property);
    }
}