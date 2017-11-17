using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NightlyCode.Core.Logs
{

    /// <summary>
	/// logs a message to a file
	/// </summary>
	public class FileLogger
    {
        // stack for logmessages which could not be written to file
        // and are written next time someone tries to log something
        readonly Queue<string> failedlog = new Queue<string>();

        readonly object lockObject = new object();
        readonly string filenamebase;

        DateTime currentday = DateTime.MinValue.Date;
        string currentfile = "";

        /// <summary>
        /// constructor which creates a log using the specified filenamebase and maximum size
        /// </summary>
        /// <param name="filename">the filenamebase of the log</param>
        public FileLogger(string filename = null)
        {
            DetermineDay = () => DateTime.Now.Date;
            OpenWriter = OpenWriterDefault;

            filenamebase = string.IsNullOrEmpty(filename) ? DetermineLogfileName() : filename;
            filenamebase = Path.GetFullPath(Environment.ExpandEnvironmentVariables(filenamebase));
        }

        /// <summary>
        /// func used to determine day
        /// </summary>
        public Func<DateTime> DetermineDay { get; set; }

        /// <summary>
        /// func used to open writer
        /// </summary>
        public Func<string, TextWriter> OpenWriter { get; set; }

        string DetermineLogfileName()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string path = Directory.GetCurrentDirectory();
            string filenbasename = "logger.log";
            if(entryAssembly == null)
                return Path.Combine(path, filenbasename);

            path = Path.GetDirectoryName(entryAssembly.Location) ?? "";
            filenbasename = Path.GetFileNameWithoutExtension(entryAssembly.CodeBase);
            filenbasename += ".log";
            return Path.Combine(path, filenbasename);
        }

        void PrepareLogfile()
        {
            string directory = Path.GetDirectoryName(filenamebase);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        TextWriter OpenWriterDefault(string filename)
        {
            PrepareLogfile();
            return new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                AutoFlush = true
            };
        }

        void DetermineCurrentFile(DateTime day)
        {
            currentday = day;
            string directory = Path.GetDirectoryName(filenamebase) ?? "./";
            currentfile = Path.Combine(directory, $"{currentday.ToString("yyyy-MM-dd")}_{Path.GetFileName(filenamebase)}");
        }

        /// <summary>
		/// Filename of the log
		/// </summary>
		public string FileName => filenamebase;

        /// <summary>
        /// Logs a log message
        /// </summary>
        /// <param name="sender">sender of log message</param>
        /// <param name="message">The message to log</param>
        /// <param name="type">message type</param>
        /// <param name="details">details for logmessage</param>
        public void Log(MessageType type, string sender, string message, string details)
        {
            if(string.IsNullOrEmpty(details))
                AppendText($"{DateTime.Now.ToString("HH:mm:ss,fff")}: {sender}, {type} - {message}");
            else
                AppendText($"{DateTime.Now.ToString("HH:mm:ss,fff")}: {sender}, {type} - {message}\r\n{details}");
            
        }

        internal void AppendText(string text)
        {
            lock (lockObject)
            {
                if (string.IsNullOrEmpty(text) && failedlog.Count == 0)
                    return;

                try
                {
                    DateTime date = DetermineDay();
                    if (date > currentday)
                        DetermineCurrentFile(date);


                    using (TextWriter writer = OpenWriter(currentfile))
                    {
                        // failed logs
                        while (failedlog.Count > 0)
                        {
                            string failedmessage = failedlog.Dequeue();
                            if (!string.IsNullOrEmpty(failedmessage))
                                writer.WriteLine(failedmessage);
                        }

                        // current message
                        if (!string.IsNullOrEmpty(text))
                            writer.WriteLine(text);
                    }
                }
                catch (Exception)
                {
                    failedlog.Enqueue(text);
                }
            }
        }

        /// <summary>
        /// flushes the logger writing all left data to disk
        /// </summary>
        public void Flush()
        {
            AppendText(null); // writes out the failedlog
        }
    }
}
