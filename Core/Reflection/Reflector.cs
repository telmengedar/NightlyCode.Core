using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Core.Reflection {

    /// <summary>
    /// helper methods used to get information about classes using reflection
    /// </summary>
    public static class Reflector {

        /// <summary>
        /// find property in instance
        /// </summary>
        /// <param name="instance">instance in which to search for property</param>
        /// <param name="property">
        /// path to property
        /// path can point to a nested property by separating childs using '.'
        /// </param>
        /// <returns>property information</returns>
        public static ReflectedProperty FindProperty(object instance, string property) {
            if(instance == null) return null;

            object currentinstance = instance;

            string[] splitproperties = property.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            PropertyInfo propertyinfo=null;

            for(int i=0;i<splitproperties.Length;++i) {
                propertyinfo = currentinstance.GetType().GetProperty(splitproperties[i]);
                if(propertyinfo == null)
                    throw new ArgumentException($"Property '{property}' not found");

                if(i >= splitproperties.Length - 1)
                    break;

                currentinstance = propertyinfo.GetValue(currentinstance, null);
                if(currentinstance == null) return null;
            }

            return new ReflectedProperty(currentinstance, propertyinfo);
        }

        /// <summary>
        /// get value of property of an instance
        /// </summary>
        /// <param name="instance">instance of which to get property value</param>
        /// <param name="property">
        /// path to property
        /// path can point to a nested property by separating childs using '.'
        /// </param>
        /// <returns>property value or null if proper</returns>
        public static object GetPropertyValue(object instance, string property) {
            ReflectedProperty rprop = FindProperty(instance, property);
            return rprop?.GetValue();
        }

        /// <summary>
        /// find all methods matching the specified name and parameter types
        /// </summary>
        /// <param name="type">type to analyse</param>
        /// <param name="name">name of method</param>
        /// <param name="parameters">parameters to match</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> FindMethod(Type type, string name, object[] parameters) {
            foreach(MethodInfo method in type.GetMethods().Where(m => m.Name == name)) {
                ParameterInfo[] methodparameters = method.GetParameters();
                if(ParametersMatching(methodparameters, parameters))
                    yield return method;
            }

            // if type is interface, base interfaces have to get analysed
            if(type.IsInterface) {
                foreach(Type baseinterface in type.GetInterfaces())
                    foreach(MethodInfo method in FindMethod(baseinterface, name, parameters))
                        yield return method;
            }
        }

        static bool ParametersMatching(ParameterInfo[] parameterinfos, object[] parameters) {
            if (parameterinfos.Length != parameters.Length)
                return false;

            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i] == null) {
                    if(parameterinfos[i].ParameterType.IsValueType)
                        return false;
                }
                else {
                    if(!parameterinfos[i].ParameterType.IsInstanceOfType(parameters[i]))
                        return false;
                }
            }

            return true;
        }
    }
}
