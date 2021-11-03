
using Discord;
using Discord.WebSocket;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using System.Threading.Tasks;

using options = ATCBot.Config.SystemMessageConfigOptions;

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
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogCustom(LogMessage message, options systemMessageOption = options.Default)
        {
            string output = $"{DateTime.Now,-19} [{message.Severity,8}] {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}{message.Message} {message.Exception}";
            AggregateLog.AppendLine(output + $"[With system config {systemMessageOption}");

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

            if (systemMessageOption == options.Default)
                return;
            //Critical messages always send, default messages never do
            else if(systemMessageOption == options.Critical || Program.config.systemMessagesConfig.Value[systemMessageOption])
            {
                //@Role - Severity - Source: Message Exception
                _ = SendSystemMessage($"{(config.botRoleId != 0 && message.Severity == LogSeverity.Critical ? $"<@&{config.botRoleId}> - " : "")}" +
                    $"**{message.Severity}** - {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}" +
                    $"{message.Message}{(message.Exception == null ? "" : $" {message.Exception.Message}")}");
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Logs a critical error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Critical and automatically announces to the system channel.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogCritical(string message, Exception e = null, string source = "", options systemMessageOption = options.Critical) => LogCustom(new LogMessage(LogSeverity.Critical, source, message, e), systemMessageOption);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Debug.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogDebug(string message, string source = "", options systemMessageOption = options.Debug) => LogCustom(new LogMessage(LogSeverity.Debug, source, message), systemMessageOption);

        /// <summary>
        /// Logs an error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Error.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogError(string message, Exception e = null, string source = "", options systemMessageOption = options.Error) => LogCustom(new LogMessage(LogSeverity.Error, source, message, e), systemMessageOption);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Info.
        /// </remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogInfo(string message, string source = "", options systemMessageOption = options.Info) => LogCustom(new LogMessage(LogSeverity.Info, source, message), systemMessageOption);

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Verbose.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogVerbose(string message, string source = "", options systemMessageOption = options.Verbose) => LogCustom(new LogMessage(LogSeverity.Verbose, source, message), systemMessageOption);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Warning.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="systemMessageOption">The type of log this is, to determine whether it should be output to the system message channel.</param>
        public static void LogWarning(string message, string source = "", options systemMessageOption = options.Warning) => LogCustom(new LogMessage(LogSeverity.Warning, source, message), systemMessageOption);

        /// <summary>
        /// Send a system message to <see cref="Config.systemMessageChannelId"/> if it is set.
        /// </summary>
        /// <param name="s">The message to send.</param>
        internal static async Task SendSystemMessage(string s)
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