using System;

namespace NightlyCode.Core.Logs {

    /// <summary>
    /// logs messages to console
    /// </summary>
    public static class ConsoleLogger {
        static readonly object loglock = new object();

        /// <summary>
        /// logs a message
        /// </summary>
        /// <param name="type">type of message to log</param>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        /// <param name="details">details of message</param>
        public static void Log(MessageType type, string sender, string message, string details) {
            lock(loglock) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(sender);

                switch(type) {
                case MessageType.Info:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                }
                Console.WriteLine(message);

                if(details == null) return;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(details);
            }
        }
    }
}