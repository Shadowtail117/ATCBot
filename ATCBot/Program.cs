using ATCBot.Commands;
using ATCBot.Structs;

using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
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
        public static DiscordSocketClient Client { get; set; }

        private CommandBuilder commandBuilder;

        private CommandHandler commandHandler;

        internal static LobbyHandler lobbyHandler;

        private static bool forceDontSaveConfig = false;

        /// <summary>
        /// Whether or not we should be updating the lobby information.
        /// </summary>
        public static bool updating = false;

        /// <summary>
        /// The current instance of the config.
        /// </summary>
        public static Config config = Config.Current;

        /// <summary>
        /// Whether or not we should immediately shutdown.
        /// </summary>
        public static bool shouldShutdown = false;

        /// <summary>
        /// Whether or not we should refresh the messages.
        /// </summary>
        public static bool shouldRefresh = false;

        /// <summary>
        /// Represents the current operational status of the bot.
        /// </summary>
        public enum Status
        {
            /// <summary>The bot is online.</summary>
            Online,
            /// <summary>The bot is offline.</summary>
            Offline,
            /// <summary>Custom status.</summary>
            Custom
        }

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
                Log.LogError("Could not load Steam config. Aborting. Press any key to exit.");
                Console.ReadKey();
                return;

            }
            try
            {
                new Program().MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Log.LogCritical($"Fatal error! Ejecting! {e.Message}", e, "Main", true);
                Environment.Exit(1);
            }
        }

        async Task MainAsync()
        {
            Client = new DiscordSocketClient();
            Client.Log += DiscordLog;
            Client.Ready += ClientReady;
            Client.Disconnected += OnDisconnected;

            await Client.LoginAsync(TokenType.Bot, config.token);
            await Client.StartAsync();

            lobbyHandler = new(this);

            _ = lobbyHandler.QueryTimer(LobbyHandler.queryToken.Token);

            while (!shouldShutdown)
                await Task.Delay(1000);
        }

        private static Task DiscordLog(LogMessage message)
        {
            Log.LogCustom(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the status to online, offline, or a custom message.
        /// </summary>
        /// <param name="status">The status type of the bot.</param>
        public static void SetStatus(Status status) => config.status = status;

        /// <summary>
        /// Sets the status to custom and sets the custom status message.
        /// </summary>
        /// <param name="status">The custom message to set.</param>
        public static void SetStatus(string status)
        {
            config.status = Status.Custom;
            config.customStatusMessage = status;
        }

        /// <summary>
        /// Update the lobby information, either editing past messages or creating new ones.
        /// </summary>
        public async Task UpdateInformation()
        {
            if (!LobbyHandler.triedLoggingIn)
                return;
            if (Client.ConnectionState == ConnectionState.Disconnected)
                await OnDisconnected(default);


            if (config.vtolLobbyChannelId == 0)
                Log.LogWarning("VTOL Lobby Channel ID is not set!", "VTOL Embed Builder");
            else
                await UpdateVtolMessage();


            if (config.jetborneLobbyChannelId == 0)
                Log.LogWarning("JBR Lobby Channel ID is not set!", "JBR Embed Builder");
            else
                await UpdateJetborneMessage();


            if (config.statusMessageChannelId == 0)
                Log.LogWarning("Status message channel ID is not set!", "Status Embed Builder");
            else
                await UpdateStatusMessage();

            shouldRefresh = false;
        }

        /// <summary>
        /// Updates the VTOL VR lobby message once.
        /// </summary>
        public static async Task UpdateVtolMessage()
        {
            var vtolEmbed = CreateVtolEmbed();

            var vtolChannel = (ISocketMessageChannel) await Client.GetChannelAsync(config.vtolLobbyChannelId);

            if (vtolChannel == null)
            {
                Log.LogWarning("VTOL Lobby Channel ID is incorrect!", "VTOL Embed Builder", true);
                return;
            }

            if (shouldRefresh)
            {
                try
                {
                    await vtolChannel.DeleteMessageAsync(config.vtolLastMessageId);
                    Log.LogInfo("Deleted VTOL message!");
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't delete VTOL message!", e, "VTOL Embed Builder", true);
                    updating = false;
                }
            }

            if (config.vtolLastMessageId != 0 && await vtolChannel.GetMessageAsync(config.vtolLastMessageId) != null)
            {
                await vtolChannel.ModifyMessageAsync(config.vtolLastMessageId, m => m.Embed = vtolEmbed.Build());
            }
            else
            {
                try
                {
                    Log.LogInfo("Couldn't find existing VTOL message, making a new one...");
                    var newMessage = await vtolChannel.SendMessageAsync(embed: vtolEmbed.Build());
                    config.vtolLastMessageId = newMessage.Id;
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't send VTOL message!", e, "VTOL Embed Builder", true);
                    updating = false;
                }
            }
        }

        /// <summary>
        /// Updates the JBR lobby message once.
        /// </summary>
        public static async Task UpdateJetborneMessage()
        {
            var jetborneEmbed = CreateJetborneEmbed();

            var jetborneChannel = (ISocketMessageChannel) await Client.GetChannelAsync(config.jetborneLobbyChannelId);

            if (jetborneChannel == null)
            {
                Log.LogWarning("JBR Lobby Channel ID is incorrect!", "JBR Embed Builder");
                return;
            }

            if (shouldRefresh)
            {
                try
                {
                    await jetborneChannel.DeleteMessageAsync(config.jetborneLastMessageId);
                    Log.LogInfo("Deleted JBR message!");
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't delete JBR message!", e, "JBR Embed Builder", true);
                    updating = false;
                }
            }

            if (config.jetborneLastMessageId != 0 && await jetborneChannel.GetMessageAsync(config.jetborneLastMessageId) != null)
            {
                await jetborneChannel.ModifyMessageAsync(config.jetborneLastMessageId, m => m.Embed = jetborneEmbed.Build());
            }
            else
            {
                try
                {
                    Log.LogInfo("Couldn't find existing JBR message, making a new one...");
                    var newMessage = await jetborneChannel.SendMessageAsync(embed: jetborneEmbed.Build());
                    config.jetborneLastMessageId = newMessage.Id;
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't send JBR message!", e, "JBR Embed Builder", true);
                    updating = false;
                }
            }
        }

        /// <summary>
        /// Updates the status message once.
        /// </summary>
        public static async Task UpdateStatusMessage()
        {
            var statusEmbed = CreateStatusEmbed();

            var statusChannel = (ISocketMessageChannel) await Client.GetChannelAsync(config.statusMessageChannelId);

            if (statusChannel == null)
            {
                Log.LogWarning("Status channel ID is incorrect!", "Status Embed Builder");
                return;
            }

            if (shouldRefresh)
            {
                try
                {
                    await statusChannel.DeleteMessageAsync(config.statusLastMessageId);
                    Log.LogInfo("Deleted JBR message!");
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't delete status message!", e, "Status Embed Builder", true);
                    updating = false;
                }
            }

            if (config.statusLastMessageId != 0 && await statusChannel.GetMessageAsync(config.statusLastMessageId) != null)
            {
                await statusChannel.ModifyMessageAsync(config.statusLastMessageId, m => m.Embed = statusEmbed.Build());
            }
            else
            {
                try
                {
                    Log.LogInfo("Couldn't find existing status message, making a new one...");
                    var newMessage = await statusChannel.SendMessageAsync(embed: statusEmbed.Build());
                    config.statusLastMessageId = newMessage.Id;
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.LogError("Couldn't send status message!", e, "Status Embed Builder", true);
                    updating = false;
                }
            }
        }


        private static EmbedBuilder CreateVtolEmbed()
        {
            EmbedBuilder vtolEmbedBuilder = new();
            vtolEmbedBuilder.WithColor(Color.DarkGrey).WithCurrentTimestamp().WithTitle("VTOL VR Lobbies:");
            if (lobbyHandler.vtolLobbies.Count - LobbyHandler.PasswordedLobbies > 0)
            {
                foreach (VTOLLobby lobby in lobbyHandler.vtolLobbies.Where(l => !l.PasswordProtected()))
                {
                    if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty || lobby.ScenarioName == string.Empty)
                    {
                        Log.LogWarning("Invalid lobby state!", "VTOL Embed Builder", true);
                        continue;
                    }
                    string content = $"{lobby.ScenarioName}\n{lobby.PlayerCount}/{lobby.MaxPlayers} Players";
                    vtolEmbedBuilder.AddField(lobby.LobbyName, content);
                }
                vtolEmbedBuilder.WithFooter($"+{LobbyHandler.PasswordedLobbies} password protected {(LobbyHandler.PasswordedLobbies == 1 ? "lobby" : "lobbies")}");
            }
            else if (LobbyHandler.PasswordedLobbies > 0)
            {
                vtolEmbedBuilder.AddField($"No public lobbies!", "Check back later!");
                vtolEmbedBuilder.WithFooter($"+{LobbyHandler.PasswordedLobbies} password protected {(LobbyHandler.PasswordedLobbies == 1 ? "lobby" : "lobbies")}");
            }
            else
                vtolEmbedBuilder.AddField("No lobbies!", "Check back later!");

            return vtolEmbedBuilder;
        }

        private static EmbedBuilder CreateJetborneEmbed()
        {
            EmbedBuilder jetborneEmbedBuilder = new();
            jetborneEmbedBuilder.WithColor(Color.DarkGrey).WithCurrentTimestamp().WithTitle("Jetborne Racing Lobbies:");
            if (lobbyHandler.jetborneLobbies.Count > 0)
            {
                foreach (JetborneLobby lobby in lobbyHandler.jetborneLobbies)
                {
                    if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty)
                    {
                        Log.LogWarning("Invalid lobby state!", "JBR Embed Builder");
                        continue;
                    }
                    string content = $"{lobby.PlayerCount} Player{(lobby.PlayerCount == 1 ? "" : "s")}\n{(lobby.CurrentLap == 0 ? "Currently In Lobby" : $"Lap { lobby.CurrentLap}/{ lobby.RaceLaps}")}";
                    jetborneEmbedBuilder.AddField(lobby.LobbyName, content);
                }
            }
            else
                jetborneEmbedBuilder.AddField("No lobbies!", "Check back later!");

            return jetborneEmbedBuilder;
        }

        private static EmbedBuilder CreateStatusEmbed() => CreateStatusEmbed(config.status == Status.Online ? "Online" : config.status == Status.Offline ? "Offline" : config.customStatusMessage);
        private static EmbedBuilder CreateStatusEmbed(string status)
        {
            EmbedBuilder statusEmbedBuilder = new();
            statusEmbedBuilder.WithColor(Color.DarkGrey).WithCurrentTimestamp().WithTitle("ATCBot Status:");
            statusEmbedBuilder.WithDescription($"**{status}**");
            statusEmbedBuilder.WithFooter("Note - this may not always be up to date.");
            return statusEmbedBuilder;
        }

        //Event methods vvv

        async Task ClientReady()
        {
            Log.LogInfo("Ready!", "Discord Client", true);
            //We check the version here so that it outputs to the system channel
            if (!await Version.CheckVersion())
            {
                Log.LogWarning($"Version mismatch! Please update ATCBot when possible. Local version: " +
                    $"{Version.LocalVersion} - Remote version: {Version.RemoteVersion}", "Version Checker", true);
            }

            commandHandler = new();

            commandBuilder = new(Client);
            Client.InteractionCreated += commandHandler.ClientInteractionCreated;
            await commandBuilder.BuildCommands();

            //Since this is loaded straight from config at this point, a status of offline means that the bot closed naturally without a custom message
            if (config.status == Status.Offline)
            {
                SetStatus(Status.Online);
                await UpdateStatusMessage();
            }
        }

        static void OnExit(object sender, EventArgs e)
        {
            if (config.status != Status.Custom)
            {
                SetStatus(Status.Offline);
            }

            Log.LogInfo("Shutting down! o7", announce: true);
            if (forceDontSaveConfig)
                return;
            Console.WriteLine("------");
            if (config.shouldSave)
            {
                Console.WriteLine("Saving config!");
                config.Save(false);
            }
            else
                Console.WriteLine("Not saving config!");

            try
            {
                UpdateStatusMessage().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.LogError("Couldn't set status to offline before quitting!", ex);
            }

            Log.SaveLog();
        }

        private Task OnDisconnected(Exception e)
        {
            Log.LogInfo("Discord has disconnected! Reason: " + e.Message, "Discord Client", true);
            WaitForReconnect();


            async void WaitForReconnect()
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                if (Client.ConnectionState == ConnectionState.Disconnected)
                {
                    Log.LogCritical("It's been 10 seconds and we haven't reconnected! Ejecting!", e, "Discord Client");
                    Environment.Exit(1);
                }
                else
                {
                    Log.LogInfo("Reconnected. As a precaution, we will restart the lobby queries.", "Discord Client", true);
                    lobbyHandler.ResetQueryTimer();
                }
            }
            return Task.CompletedTask;
        }
    }
}
