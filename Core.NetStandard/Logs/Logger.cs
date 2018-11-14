using System;
using System.Diagnostics;

namespace NightlyCode.Core.Logs
{

    /// <summary>
    /// manages logging
    /// </summary>
    public class Logger {
        static bool consoleenabled;

        /// <summary>
        /// triggered when a new logmessage was generated
        /// </summary>
        public static event Action<MessageType, string, string, string> Message;

        static void OnMessage(MessageType type, string sender, string message, string details) {
            Message?.Invoke(type, sender, message, details);
        }

        static string GetSender(object sender) {
            Debug.Assert(sender != null);
            if(sender is string)
                return (string)sender;
            if(sender is Type)
                return ((Type)sender).Name;

            return sender.GetType().Name;
        }

        /// <summary>
        /// logs an info
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        /// <param name="details">optional details</param>
        public static void Info(object sender, string message, string details = null) {
            OnMessage(MessageType.Info, GetSender(sender), message, details);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        public static void Warning(object sender, string message) {
            OnMessage(MessageType.Warning, GetSender(sender), message, null);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        /// <param name="details">details of warning</param>
        public static void Warning(object sender, string message, string details) {
            OnMessage(MessageType.Warning, GetSender(sender), message, details);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        /// <param name="error">error inducing warning</param>
        public static void Warning(object sender, string message, Exception error) {
            OnMessage(MessageType.Warning, GetSender(sender), message, error.ToString());
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">message content</param>
        /// <param name="error">detailed exception</param>
        public static void Error(object sender, string message, Exception error = null) {
            OnMessage(MessageType.Error, GetSender(sender), message, error?.ToString());
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="sender">sender of message</param>
        /// <param name="message">short description of error</param>
        /// <param name="details">error details</param>
        public static void Error(object sender, string message, string details) {
            OnMessage(MessageType.Error, GetSender(sender), message, details);
        }

        /// <summary>
        /// enables logging to console
        /// </summary>
        public static void EnableConsoleLogging() {
            if(consoleenabled)
                return;

            consoleenabled = true;
            Message += ConsoleLogger.Log;
        }

    }
}
