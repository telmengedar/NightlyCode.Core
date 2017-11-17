using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NightlyCode.Core.Helpers {

    /// <summary>
    /// extensions for directories
    /// </summary>
    public static class PathExtensions {

        /// <summary>
        /// get all subdirectory names of the specified directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllSubDirectories(string directory) {
            directory = Path.GetFullPath(directory);
            yield return directory;
            foreach(string subdirectory in Directory.GetDirectories(directory))
                foreach(string dir in GetAllSubDirectories(subdirectory))
                    yield return dir;
        }

        /// <summary>
        /// get directory of currently executing application
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationDirectory() {
            // one of these assemblies is always set
            Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location);
        }

        /// <summary>
        /// copies a directory
        /// </summary>
        /// <param name="sourcedirectory">directory to be copied</param>
        /// <param name="targetdirectoryname">full path to copied directory</param>
        /// <returns></returns>
        public static void Copy(string sourcedirectory, string targetdirectoryname) {
            sourcedirectory = sourcedirectory.Replace('/', '\\');
            if(!Directory.Exists(targetdirectoryname))
                Directory.CreateDirectory(targetdirectoryname);

            foreach(string filename in Directory.GetFiles(sourcedirectory)) {
                string file = Path.GetFileName(filename);
                if(string.IsNullOrEmpty(file))
                    continue;
                File.Copy(filename, Path.Combine(targetdirectoryname, file), true);
            }

            foreach(string directory in Directory.GetDirectories(sourcedirectory)) {
                int offset = sourcedirectory.EndsWith("\\") ? 0 : 1;
                string target = directory.Substring(sourcedirectory.Length + offset);
                Copy(directory, target);
            }
        }
    }
}