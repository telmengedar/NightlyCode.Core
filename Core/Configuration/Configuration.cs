using System;
using System.ComponentModel;
using System.Reflection;
using NightlyCode.Core.Conversion;
using NightlyCode.Core.Logs;

namespace NightlyCode.Core.Configuration {

    /// <summary>
    /// base configuration which manages automatic persistence
    /// </summary>
    public class Configuration : IConfiguration {
        readonly IConfigurationAccess access;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="access"></param>
        public Configuration(IConfigurationAccess access) {
            this.access = access;
            LoadConfiguration();
            PropertyChanged += ConfigurationChanged;
        }

        void LoadConfiguration() {
            foreach(PropertyInfo property in GetType().GetProperties(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)) {
                if(access.ContainsValue(property.Name)) {
                    try {
                        object value = access.GetConfigValue<object>(property.Name);
                        property.SetValue(this, Converter.Convert(value, property.PropertyType), null);
                    }
                    catch(Exception e) {
                        Logger.Info(this, $"Unable to read config value for {property.Name}: {e}");
                        RevertToDefault(property.Name);
                    }
                }
                else
                    RevertToDefault(property.Name);
            }
        }

        void ConfigurationChanged(object sender, PropertyChangedEventArgs e) {
            PropertyInfo property = GetProperty(e.PropertyName);

            DefaultValueAttribute defaultvalue = DefaultValueAttribute.Get(property);
            if(defaultvalue == null)
                return;

            access.SetConfigValue(e.PropertyName, defaultvalue.Value);
        }

        PropertyInfo GetProperty(string property) {
            PropertyInfo propertyinfo = GetType().GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(propertyinfo == null)
                throw new ConfigurationException($"{property} not found in {GetType().Name}");
            return propertyinfo;
        }

        /// <summary>
        /// reverts the property to its default value
        /// </summary>
        /// <param name="property">property to revert</param>
        public void RevertToDefault(string property) {
            PropertyInfo propertyinfo = GetProperty(property);

            DefaultValueAttribute defaultvalue = DefaultValueAttribute.Get(propertyinfo);
            if(defaultvalue == null) {
                Logger.Info(this, $"Unable to find default for {property}");
                return;
            }

            propertyinfo.SetValue(this, Converter.Convert(defaultvalue.Value, propertyinfo.PropertyType), null);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// used to inform about property value changes
        /// </summary>
        /// <param name="property">name of property which has changed</param>
        protected void OnPropertyChanged(string property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}