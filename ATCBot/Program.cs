using ATCBot.Commands;
using ATCBot.Structs;

using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

namespace ATCBot
{
    /// <summary>
    /// Main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The Steam game ID of VTOL VR.
        /// </summary>
        public const int vtolID = 667970;

        /// <summary>
        /// The Steam game ID of Jetborne Racing.
        /// </summary>
        public const int jetborneID = 1397650;

        /// <summary>
        /// The program's instance of the bot.
        /// </summary>
        public static DiscordSocketClient client;

        private CommandBuilder commandBuilder;

        private CommandHandler commandHandler;

        private LobbyHandler lobbyHandler;

        private static bool forceDontSaveConfig = false;

        /// <summary>
        /// Whether or not we should be updating the lobby information.
        /// </summary>
        public static bool shouldUpdate = false;

        /// <summary>
        /// The current instance of the config.
        /// </summary>
        public static Config config = Config.config;

        /// <summary>
        /// Whether or not we should immediately shutdown.
        /// </summary>
        public static bool shouldShutdown = false;

        /// <summary>
        /// Whether or not we should refresh the messages.
        /// </summary>
        public static bool shouldRefresh = false;

        static void Main(string[] args)
        {
            //Stuff to set up the console
            Console.Title = "ATCBot v." + Version.LocalVersion;
            Console.WriteLine($"Booting up ATCBot version {Version.LocalVersion}.");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            config = new Config();

            if (!config.Load(out config))
            {
                forceDontSaveConfig = true;
                Console.WriteLine("Couldn't load config. Aborting. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            if (!SteamConfig.Load())
            {
                Console.WriteLine("Could not load Steam config. Aborting. Press any key to exit.");
                Console.ReadKey();
                return;

            }

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += DiscordLog;
            client.Ready += ClientReady;

            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();

            lobbyHandler = new(this);

            await lobbyHandler.QueryTimer();

            await Task.Delay(-1);
        }

        private static Task DiscordLog(LogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Info.
        /// </remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogInfo(string message, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Info, source, message), announce);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Warning.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogWarning(string message, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Warning, source, message), announce);

        /// <summary>
        /// Logs an error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Error.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogError(string message, Exception e = null, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Error, source, message, e), announce);

        /// <summary>
        /// Logs a critical error message along with an optional exception.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Critical.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="e">The exception to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogCritical(string message, Exception e = null, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Error, source, message, e), announce);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Debug.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogDebug(string message, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Debug, source, message), announce);

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <remarks>Automatically assigns a <see cref="LogSeverity"/> of Verbose.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void LogVerbose(string message, string source = "", bool announce = false) => Log(new LogMessage(LogSeverity.Verbose, source, message), announce);

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <remarks>Use when another logging method is not precise enough.</remarks>
        /// <param name="message">The message to be logged.</param>
        /// <param name="announce">Whether or not to announce the message to <see cref="Config.systemMessageChannelId"/>.</param>
        public static void Log(LogMessage message, bool announce = false)
        {
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
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}{message.Message} {message.Exception}");
            if (announce) _ = SendSystemMessage($"**{message.Severity}** - {(message.Source.Equals(string.Empty) ? "" : $"{ message.Source}: ")}{message.Message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Send a system message to <see cref="Config.systemMessageChannelId"/> if it is set.
        /// </summary>
        /// <param name="s">The message to send.</param>
        public static async Task SendSystemMessage(string s)
        {
            if(config.systemMessageChannelId == 0)
            {
                LogInfo("Tried announcing a message but the system channel ID is not set.");
                return;
            }

            var systemChannel = (ISocketMessageChannel)await client.GetChannelAsync(config.systemMessageChannelId);

            if (systemChannel == null)
            {
                LogInfo("Tried announcing a message but couldn't find a channel.");
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

        /// <summary>
        /// Update the lobby information, either editing past messages or creating new ones.
        /// </summary>
        public async Task UpdateLobbyInformation()
        {
            if (!LobbyHandler.triedLoggingIn) return;

            //VTOL lobbies
            if (config.vtolLobbyChannelId == 0)
            {
                LogWarning("VTOL Lobby Channel ID is not set!", "VTOL Embed Builder");
            }
            else
            {
                EmbedBuilder vtolEmbedBuilder = new();
                vtolEmbedBuilder.WithColor(Color.DarkGrey).WithCurrentTimestamp().WithTitle("VTOL VR Lobbies:");
                if (lobbyHandler.vtolLobbies.Count > 0)
                {
                    foreach (VTOLLobby lobby in lobbyHandler.vtolLobbies)
                    {
                        if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty || lobby.ScenarioText == string.Empty)
                        {
                            LogWarning("Invalid lobby state!", "VTOL Embed Builder");
                            continue;
                        }
                        string content = $"{lobby.ScenarioText}\n{lobby.MemberCount} Players";
                        vtolEmbedBuilder.AddField(lobby.LobbyName, content);
                    }
                    if(VTOLLobby.passwordLobbies > 0)
                    {
                        vtolEmbedBuilder.WithFooter($"+{VTOLLobby.passwordLobbies} password protected {(VTOLLobby.passwordLobbies == 1 ? "lobby" : "lobbies")}");
                    }
                }
                else vtolEmbedBuilder.AddField("No lobbies!", "Check back later!");

                var vtolChannel = (ISocketMessageChannel)await client.GetChannelAsync(config.vtolLobbyChannelId);

                if (vtolChannel == null)
                {
                    LogWarning("VTOL Lobby Channel ID is incorrect!", "VTOL Embed Builder", true);
                    return;
                }

                if(shouldRefresh)
                {
                    try
                    {
                        await vtolChannel.DeleteMessageAsync(config.vtolLastMessageId);
                        LogInfo("Deleted VTOL message!");
                    }
                    catch (Discord.Net.HttpException e)
                    {
                        LogError("Couldn't delete VTOL message!", e, "VTOL Embed Builder", true);
                        shouldUpdate = false;
                    }
                }

                if (config.vtolLastMessageId != 0 && await vtolChannel.GetMessageAsync(config.vtolLastMessageId) != null)
                {
                    await vtolChannel.ModifyMessageAsync(config.vtolLastMessageId, m => m.Embed = vtolEmbedBuilder.Build());
                }
                else
                {
                    try
                    {
                        LogInfo("Couldn't find existing VTOL message, making a new one...");
                        var newMessage = await vtolChannel.SendMessageAsync(embed: vtolEmbedBuilder.Build());
                        config.vtolLastMessageId = newMessage.Id;
                    }
                    catch (Discord.Net.HttpException e)
                    {
                        LogError("Couldn't send VTOL message!", e, "VTOL Embed Builder", true);
                        shouldUpdate = false;
                    }
                }

            }

            //JBR lobbies
            if (config.jetborneLobbyChannelId == 0)
            {
                LogWarning("JBR Lobby Channel ID is not set!", "JBR Embed Builder");
            }
            else
            {
                EmbedBuilder jetborneEmbedBuilder = new();
                jetborneEmbedBuilder.WithColor(Color.DarkGrey).WithCurrentTimestamp().WithTitle("Jetborne Racing Lobbies:");
                if (lobbyHandler.jetborneLobbies.Count > 0)
                {
                    foreach (JetborneLobby lobby in lobbyHandler.jetborneLobbies)
                    {
                        if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty)
                        {
                            LogWarning("Invalid lobby state!", "JBR Embed Builder");
                            continue;
                        }
                        string content = $"{lobby.MemberCount} Players\nLap" + lobby.CurrentLap == string.Empty ? "Currently In Lobby" : $"Lap {lobby.CurrentLap}/{lobby.RaceLaps}";
                        jetborneEmbedBuilder.AddField(lobby.LobbyName, content);
                    }
                }
                else jetborneEmbedBuilder.AddField("No lobbies!", "Check back later!");

                var jetborneChannel = (ISocketMessageChannel)await client.GetChannelAsync(config.jetborneLobbyChannelId);

                if (jetborneChannel == null)
                {
                    LogWarning("JBR Lobby Channel ID is incorrect!", "JBR Embed Builder");
                    return;
                }

                if (shouldRefresh)
                {
                    try
                    {
                        await jetborneChannel.DeleteMessageAsync(config.jetborneLastMessageId);
                        LogInfo("Deleted JBR message!");
                    }
                    catch (Discord.Net.HttpException e)
                    {
                        LogError("Couldn't delete JBR message!", e, "JBR Embed Builder", true);
                        shouldUpdate = false;
                    }
                }

                if (config.jetborneLastMessageId != 0 && await jetborneChannel.GetMessageAsync(config.jetborneLastMessageId) != null)
                {
                    await jetborneChannel.ModifyMessageAsync(config.jetborneLastMessageId, m => m.Embed = jetborneEmbedBuilder.Build());
                }
                else
                {
                    try
                    {
                        LogInfo("Couldn't find existing JBR message, making a new one...");
                        var newMessage = await jetborneChannel.SendMessageAsync(embed: jetborneEmbedBuilder.Build());
                        config.jetborneLastMessageId = newMessage.Id;
                    }
                    catch (Discord.Net.HttpException e)
                    {
                        LogError("Couldn't send JBR message!", e, "JBR Embed Builder", true);
                        shouldUpdate = false;
                    }
                }

                shouldRefresh = false;
            }
        }

        //Event methods vvv

        async Task ClientReady()
        {
            //We check the version here so that it outputs to the system channel
            if (!await Version.CheckVersion())
            {
                LogWarning($"Version mismatch! Please update ATCBot when possible. Local version: " +
                    $"{Version.LocalVersion} - Remote version: {Version.RemoteVersion}", "Version Checker", true);
            }

            commandHandler = new();
            commandBuilder = new(client);
            client.InteractionCreated += commandHandler.ClientInteractionCreated;
            await commandBuilder.BuildCommands();
        }

        static void OnExit(object sender, EventArgs e)
        {
            Program.LogInfo("Shutting down! o7", announce: true);
            if (forceDontSaveConfig) return;
            Console.WriteLine("------");
            if (config.shouldSave)
            {
                Console.WriteLine("Saving config!");
                config.Save(false);
            }
            else
                Console.WriteLine("Not saving config!");

            Console.WriteLine("Press any key to exit. Goodbye!");
            Console.ReadKey();
        }
    }
}
