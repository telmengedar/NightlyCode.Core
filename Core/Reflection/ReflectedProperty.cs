using System.Reflection;

namespace NightlyCode.Core.Reflection {

    /// <summary>
    /// property of an instance
    /// </summary>
    public class ReflectedProperty {
        readonly PropertyInfo property;
        readonly object instance;

        /// <summary>
        /// creates a new <see cref="ReflectedProperty"/>
        /// </summary>
        /// <param name="instance">instance of property</param>
        /// <param name="property">property info</param>
        public ReflectedProperty(object instance, PropertyInfo property) {
            this.instance = instance;
            this.property = property;
        }

        /// <summary>
        /// property information
        /// </summary>
        public PropertyInfo Property => property;

        /// <summary>
        /// instance used to access property
        /// </summary>
        public object Instance => instance;

        /// <summary>
        /// get value from property
        /// </summary>
        /// <returns>value stored in property</returns>
        public object GetValue() {
            return property.GetValue(instance, null);
        }

        /// <summary>
        /// set value of property
        /// </summary>
        /// <param name="value">value to set</param>
        public void SetValue(object value) {
            property.SetValue(instance, value, null);
        }
    }
}
