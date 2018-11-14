using System;

namespace NightlyCode.Core.Logs {

    /// <summary>
    /// logs a message to a single console line
    /// </summary>
    public class SingleLineLogger {

        /// <summary>
        /// logs the message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="details"></param>
        public static void Log(MessageType type, string sender, string message, string details) {
            Console.Write($"\r{message}");
        }
    }
}