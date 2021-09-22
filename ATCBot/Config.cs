using Newtonsoft.Json;

using System;
using System.IO;

namespace ATCBot
{
    /// <summary>
    /// Class to show the bot's config. Loaded when the project is started and automatically saved when it closes if <cref>shouldSave</cref> is true.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// A global config to use for this instance of the bot.
        /// </summary>
        public static Config config;

        /// <summary>
        /// The bot's token. Loaded externally from <see cref="saveFile"/>.
        /// </summary>
        [JsonIgnore]
        public string token;

        /// <summary>
        /// Whether or not the config should save to disk when the program exits.
        /// </summary>
        [JsonIgnore]
        public bool shouldSave = true;

        /// <summary>
        /// The user ID of the owner of the bot. Required for some commands. Must be set manually in the .cfg.
        /// </summary>
        public ulong botOwnerId;

        /// <summary>
        /// The Discord channel ID to post the VTOL lobby information in.
        /// </summary>
        public ulong vtolLobbyChannelId;

        /// <summary>
        /// The Discord message ID of the last VTOL lobby information posting.
        /// </summary>
        public ulong vtolLastMessageId;

        /// <summary>
        /// The Discord channel ID to post the Jetborne lobby information in.
        /// </summary>
        public ulong jetborneLobbyChannelId;

        /// <summary>
        /// The Discord message ID of the last Jetborne lobby information posting.
        /// </summary>
        public ulong jetborneLastMessageId;

        /// <summary>
        /// The Discord channel ID to send system messages to.
        /// </summary>
        public ulong systemMessageChannelId;

        /// <summary>
        /// Whether or not the bot should build commands.
        /// </summary>
        /// <remarks>Will automatically set itself to false after building, edit the config directly to re-enable.</remarks>
        public bool shouldBuildCommands = true;

        /// <summary>
        /// The time in seconds to wait between server updates.
        /// </summary>
        public int delay = 5;

        /// <summary>
        /// The time in seconds for steam to timeout.
        /// </summary>
        public int steamTimeout = 1;

        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Config");
        private static readonly string saveFile = Path.Combine(saveDirectory, @"config.cfg");

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
            return true; //We cannot check if the token is actually valid here
        }
    }
}
