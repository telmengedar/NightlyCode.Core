using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Core.ComponentModel {

    /// <summary>
    /// provides access to resources
    /// </summary>
    public static class ResourceAccessor {
         
        /// <summary>
        /// determines whether the specified resource exists
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourcepath"></param>
        /// <returns></returns>
        public static bool ContainsResource(Assembly assembly, string resourcepath) {
            return assembly.GetManifestResourceNames().Contains(resourcepath);
        }

        /// <summary>
        /// lists resource names contained in the specified resource path
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourcepath"></param>
        /// <returns></returns>
        public static IEnumerable<string> ListResources(Assembly assembly, string resourcepath) {
            string identifier = resourcepath + ".";
            return assembly.GetManifestResourceNames().Where(r => r.StartsWith(identifier));
        }

        /// <summary>
        /// get resource from the current assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourcepath"></param>
        /// <returns></returns>
        public static T GetResource<T>(string resourcepath) {
            return GetResource<T>(Assembly.GetCallingAssembly(), resourcepath);
        }

        /// <summary>
        /// get resource relative to host object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="relativepath"></param>
        /// <returns></returns>
        public static T GetResource<T>(object host, string relativepath) {
            if(host == null)
                throw new ArgumentNullException(nameof(host));

            Type t = host.GetType();
            return GetResource<T>(t.Assembly, t.Namespace + "." + relativepath);
        }

        /// <summary>
        /// get a resource from an assembly
        /// </summary>
        /// <typeparam name="T">type of resource content</typeparam>
        /// <param name="assembly">assembly where the resource is stored</param>
        /// <param name="resourcepath">full path to the resource (namespace + resourcename)</param>
        /// <returns></returns>
        public static T GetResource<T>(Assembly assembly, string resourcepath) {
            Stream stream=assembly.GetManifestResourceStream(resourcepath);
            if(stream == null)
                throw new InvalidOperationException($"resource '{resourcepath}' in assembly '{assembly}' not found\r\nAvailable resources in assembly:\r\n{string.Join("\r\n", assembly.GetManifestResourceNames())}");

            if(typeof(T) == typeof(byte[])) {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return (T)(object)bytes;
            }

            if(typeof(T)==typeof(string))
                using(StreamReader sr = new StreamReader(stream))
                    return (T)(object)sr.ReadToEnd();

            if(typeof(T) == typeof(Stream))
                return (T)(object)stream;

            throw new InvalidOperationException("Invalid data type");
        }
    }
}