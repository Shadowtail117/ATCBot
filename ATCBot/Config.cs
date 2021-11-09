using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace ATCBot
{
    /// <summary>
    /// Class to show the bot's config. Loaded when the project is started and automatically saved whenever a config value is changed.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// A global config to use for this instance of the bot.
        /// </summary>
        public static Config Current { get; set; }

        /// <summary>
        /// The bot's token. Loaded externally from <see cref="saveFile"/>.
        /// </summary>
        [JsonIgnore]
        public ConfigValue<string> token;

        /// <summary>
        /// The current verbosity of the logs. Not to be confused with <see cref="systemMessagesConfig"/>.
        /// </summary>
        public ConfigValue<Log.LogVerbosity> logVerbosity;

        /// <summary>
        /// Whether or not to automatically begin queries when the Discord client is ready.
        /// </summary>
        public ConfigValue<bool> autoQuery;

        /// <summary>
        /// The custom status message to be saved between sessions.
        /// </summary>
        public ConfigValue<string> customStatusMessage;

        /// <summary>
        /// The last status message of the program, used to determine what to set upon restarting.
        /// </summary>
        public ConfigValue<Program.Status> status;

        /// <summary>
        /// The ID of the role required to use restricted bot commands. If not set, defaults to Manage Server.
        /// </summary>
        public ConfigValue<ulong> botRoleId;

        /// <summary>
        /// The user ID of the owner of the bot. Required for some commands. Must be set manually in the .cfg.
        /// </summary>
        public ConfigValue<ulong> botOwnerId;

        /// <summary>
        /// The Discord channel ID to post the VTOL lobby information in.
        /// </summary>
        public ConfigValue<ulong> vtolLobbyChannelId;

        /// <summary>
        /// The Discord message ID of the last VTOL lobby information posting.
        /// </summary>
        public ConfigValue<ulong> vtolLastMessageId;

        /// <summary>
        /// The Discord channel ID to post the Jetborne lobby information in.
        /// </summary>
        public ConfigValue<ulong> jetborneLobbyChannelId;

        /// <summary>
        /// The Discord message ID of the last Jetborne lobby information posting.
        /// </summary>
        public ConfigValue<ulong> jetborneLastMessageId;

        /// <summary>
        /// The Discord channel ID to post status messages to.
        /// </summary>
        public ConfigValue<ulong> statusMessageChannelId;

        /// <summary>
        /// The Discord message ID of the last status message.
        /// </summary>
        public ConfigValue<ulong> statusLastMessageId;

        /// <summary>
        /// The Discord channel ID to send system messages to.
        /// </summary>
        public ConfigValue<ulong> systemMessageChannelId;

        /// <summary>
        /// Whether or not the bot should build commands.
        /// </summary>
        /// <remarks>Will automatically set itself to false after building, edit the config directly to re-enable.</remarks>
        public ConfigValue<bool> shouldBuildCommands = true;

        /// <summary>
        /// The time in seconds to wait between server updates.
        /// </summary>
        public ConfigValue<int> delay = 5;

        /// <summary>
        /// The time in seconds for steam to timeout.
        /// </summary>
        public ConfigValue<int> steamTimeout = 1;

        /// <summary>
        /// A set of configuration options for what system messages should be sent.
        /// </summary>
        public ConfigValue<Dictionary<SystemMessageConfigOptions, bool>> systemMessagesConfig = defaultSystemMessageConfigOptions;

        /// <summary>
        /// Represents all available configuration options for system messages.
        /// </summary>
        public enum SystemMessageConfigOptions
        {
            /// <summary>
            /// For logs that should intentionally never be output.
            /// </summary>
            Silent,

            /// <summary>
            /// All logs that don't match any other categories.
            /// </summary>
            Default,

            /// <summary>
            /// Sent whenever the bot/SteamKit account connect or disconnect.
            /// </summary>
            ConnectionStatus,

            /// <summary>
            /// Sent whenever something happend regarding queries.
            /// </summary>
            Queries,

            /// <summary>
            /// Sent whenever a command is received.
            /// </summary>
            CommandReceived,

            /// <summary>
            /// Sent if the watchdog detects a skipped heartbeat, instead of only when it flatlines.
            /// </summary>
            WatchdogWarnings,

            /// <summary>
            /// Sent for all informational messages.
            /// </summary>
            Info,

            /// <summary>
            /// Sent for all warning messages.
            /// </summary>
            Warning,

            /// <summary>
            /// Sent for all errors. Critical errors are always sent.
            /// </summary>
            Error,

            /// <summary>
            /// Sent for all verbose messages.
            /// </summary>
            Verbose,

            /// <summary>
            /// Sent for all debug messages.
            /// </summary>
            Debug,

            /// <summary>
            /// Internal value for system-critical messages that must be sent. Should never be disabled.
            /// </summary>
            Critical
        }

        /// <summary>
        /// Represents a singular config value of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>Will automatically save <see cref="Current"/> to disk when edited.</remarks>
        /// <typeparam name="T">The type of value to store.</typeparam>
        public struct ConfigValue<T>
        {
            private T _value;

            /// <summary>
            /// The value of this config value.
            /// </summary>
            /// <remarks>Will automatically save <see cref="Current"/> to disk when edited.</remarks>
            public T Value { get => _value; set
                {
                    Program.config?.OnValueChanged();
                    _value = value;
                }
            }

            /// <summary>
            /// Returns the stored value of type <typeparamref name="T"/>.
            /// </summary>
            public static implicit operator T(ConfigValue<T> cv) => cv.Value;

            /// <summary>
            /// Creates a new <see cref="ConfigValue{T}"/> from <paramref name="value"/>.
            /// </summary>
            public static implicit operator ConfigValue<T>(T value) => new(value);

            /// <summary>
            /// Returns <see cref="Value"/> as a string.
            /// </summary>
            public override string ToString() => Value.ToString();

            /// Creates a new <see cref="ConfigValue{T}"/> from <paramref name="value"/>.
            public ConfigValue(T value, bool edit = true)
            {
                //This is done to get around needing to assign all members before using `this`
                _value = value;
                Value = value;
            }
        }

        private void OnValueChanged()
        {
            if (loading)
                return;
            Save(true);
        }

        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Config");
        private static readonly string saveFile = Path.Combine(saveDirectory, @"config.cfg");

        private static bool loading;

        private static Dictionary<SystemMessageConfigOptions, bool> defaultSystemMessageConfigOptions = new Dictionary<SystemMessageConfigOptions, bool>()
        {
            {SystemMessageConfigOptions.Silent, false },
            {SystemMessageConfigOptions.Default, false },
            {SystemMessageConfigOptions.Critical, true },

            {SystemMessageConfigOptions.ConnectionStatus, true },
            {SystemMessageConfigOptions.Queries, true },
            {SystemMessageConfigOptions.CommandReceived, true },
            {SystemMessageConfigOptions.WatchdogWarnings, false },
            {SystemMessageConfigOptions.Info, true },
            {SystemMessageConfigOptions.Warning, true },
            {SystemMessageConfigOptions.Error, true },
            {SystemMessageConfigOptions.Verbose, false },
            {SystemMessageConfigOptions.Debug, false }

        };

        /// <summary>
        /// Saves the current config to <see cref="saveFile"/>.
        /// </summary>
        /// <remarks>Does NOT save the token even if it is loaded in the config.</remarks>
        /// <param name="silent">Whether or not we should output to the console.</param>
        public void Save(bool silent = false)
        {

            if (!silent) Console.WriteLine($"Saving configuration to \"{saveDirectory}\".");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            File.WriteAllText(saveFile, JsonConvert.SerializeObject(this, Formatting.Indented));
            if (!silent) Console.WriteLine($"Finished saving configuration to \"{saveDirectory}\".");
        }

        /// <summary>
        /// Loads the current config from <see cref="saveFile"/>.
        /// </summary>
        /// <param name="config">The instance to load to.</param>
        /// <returns>Whether or not the load was successful.</returns>
        public bool Load(out Config config)
        {
            loading = true;
            Console.WriteLine($"Loading configuration from \"{saveDirectory}\".");
            if (!Directory.Exists(saveDirectory) || !File.Exists(saveFile))
            {
                Console.WriteLine("Configuration file does not exist! Creating new one.");
                Save(true);
            }

            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(saveFile));

            if (!File.Exists(Path.Combine(saveDirectory, @"token.txt")))
            {
                Console.WriteLine("token.txt does not exist! Creating new one, please add in your token.");
                File.WriteAllText(Path.Combine(saveDirectory, @"token.txt"), "Put your token here");
                return false;
            }
            string token = File.ReadAllText(Path.Combine(saveDirectory, @"token.txt"));
            if (token == "Put your token here") //The default value
            {
                Console.WriteLine("Token has not been set! Aborting.");
                return false;
            }
            config.token = token;
            loading = false;
            return true; //We cannot check if the token is actually valid here
        }
    }
}
