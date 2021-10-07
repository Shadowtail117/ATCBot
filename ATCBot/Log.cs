
using Discord;
using Discord.WebSocket;

using System;
using System.IO;
using System.Text;

using System.Threading.Tasks;

namespace ATCBot
{
    /// <summary>
    /// Represents logging and system messaging tools.
    /// </summary>
    public static class Log
    {
        private static bool warnedNoSystemChannel = false;

        private static Config config = Program.config;

        /// <summary>
        /// Holds all logs for dumping purposes.
        /// </summary>
        /// <remarks>Stores ALL logs regardless of verbosity settings.</remarks>
        public static StringBuilder AggregateLog { get; private set; } = new();

        /// <summary>
        /// The verbosity of logs to show.
        /// </summary>
        public enum LogVerbosity
        {
            /// <summary>Does not show debug or verbose logs.</summary>
            Normal,
            /// <summary>Shows verbose logs but not debug logs.</summary>
            Verbose,
            /// <summary>Shows verbose and debug logs.</summary>
            Debug
        }

        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Log");
        private static string SaveFilePath
        {
            get
            {
                DateTime date = DateTime.Now;
                return Path.Combine(saveDirectory, $"Log {date.Month} {date.Day} {date.Year}  {date.Hour} {date.Minute}.txt");
            }
        }


        /// <summary>
        /// Saves <see cref="AggregateLog"/> to disk.
        /// </summary>
        public static void SaveLog()
        {
            LogInfo($"Dumping logs to \"{SaveFilePath}\".");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            File.WriteAllText(SaveFilePath, AggregateLog.ToString());
        }

        /// <summary>
        /// Logs a blank line to the console for ease of reading.
        /// </summary>
        public static void LogBlank()
        {
            Console.WriteLine();
            AggregateLog.AppendLine();
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <remarks>Use when another logging method is not precise enough.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogCustom(LogMessage message, bool announce = false)
        {
            string output = $"{DateTime.Now,-19} [{message.Severity,8}] {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}{message.Message} {message.Exception}";
            AggregateLog.AppendLine(output);

            if (config.logVerbosity == LogVerbosity.Normal)
            {
                if (message.Severity == LogSeverity.Verbose || message.Severity == LogSeverity.Debug)
                    return;
            }
            if (config.logVerbosity == LogVerbosity.Verbose)
            {
                if (message.Severity == LogSeverity.Debug)
                    return;
            }

            Console.ForegroundColor = message.Severity switch
            {
                LogSeverity.Critical => ConsoleColor.Red,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Info => ConsoleColor.White,
                LogSeverity.Verbose => ConsoleColor.DarkGray,
                LogSeverity.Debug => ConsoleColor.DarkGray,
                _ => throw new ArgumentException("Invalid severity!")
            };
            Console.WriteLine(output);
            if (announce)
                _ = SendSystemMessage($"{(config.botRoleId != 0 && message.Severity == LogSeverity.Critical ? $"<@&{config.botRoleId}> - " : "")}**{message.Severity}** - {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}{message.Message}{(message.Exception == null ? "" : $" {message.Exception.Message}")}");
            Console.ResetColor();
        }

        /// <summary>
        /// Logs a critical error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Critical and automatically announces to the system channel.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogCritical(string message, Exception e = null, string source = "", bool announce = true) => LogCustom(new LogMessage(LogSeverity.Error, source, message, e), announce);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Debug.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogDebug(string message, string source = "", bool announce = false) => LogCustom(new LogMessage(LogSeverity.Debug, source, message), announce);

        /// <summary>
        /// Logs an error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Error.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogError(string message, Exception e = null, string source = "", bool announce = false) => LogCustom(new LogMessage(LogSeverity.Error, source, message, e), announce);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Info.
        /// </remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogInfo(string message, string source = "", bool announce = false) => LogCustom(new LogMessage(LogSeverity.Info, source, message), announce);

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Verbose.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogVerbose(string message, string source = "", bool announce = false) => LogCustom(new LogMessage(LogSeverity.Verbose, source, message), announce);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Warning.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogWarning(string message, string source = "", bool announce = false) => LogCustom(new LogMessage(LogSeverity.Warning, source, message), announce);

        /// <summary>
        /// Send a system message to <see cref="Config.systemMessageChannelId"/> if it is set.
        /// </summary>
        /// <param name="s">The message to send.</param>
        public static async Task SendSystemMessage(string s)
        {
            if (config.systemMessageChannelId == 0 && !warnedNoSystemChannel)
            {
                LogInfo("Tried announcing a message but the system channel ID is not set.");
                warnedNoSystemChannel = true;
                return;
            }

            var systemChannel = (ISocketMessageChannel) await Program.Client.GetChannelAsync(config.systemMessageChannelId);

            if (systemChannel == null)
            {
                LogWarning("Tried announcing a message but couldn't find a channel.");
                return;
            }

            try
            {
                await systemChannel.SendMessageAsync(s);
            }
            catch (Discord.Net.HttpException e)
            {
                LogError("Couldn't send system message!", e);
            }
        }
    }
}