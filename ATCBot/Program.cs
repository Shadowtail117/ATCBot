using ATCBot.Commands;
using ATCBot.Structs;

using Discord;
using Discord.WebSocket;

using Steamworks;
using Steamworks.Data;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot
{
    public class Program
    {
        public const int vtolID = 667970;
        public const int jetborneID = 1397650;

        private ulong vtolLobbyMessageId;
        private ulong jetborneLobbyMessageId;

        public DiscordSocketClient client;

        private CommandBuilder commandBuilder;

        private CommandHandler commandHandler;

        private LobbyHandler lobbyHandler;

        private static bool forceDontSaveConfig = false;
        
        public static bool shouldUpdate = false;

        public static Config config = Config.config;

        public static bool shouldShutdown = false;

        static void Main(string[] args)
        {
            //Stuff to set up the console
            Console.Title = "ATCBot v." + Config.version;
            Console.WriteLine($"Booting up ATCBot version {Config.version}.");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            Steam.SetupSteam();
            config = new Config();

            if (!config.Load(out config))
            {
                forceDontSaveConfig = true;
                Console.WriteLine("Couldn't load config. Aborting. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            client.Ready += ClientReady;

            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();

            lobbyHandler = new(this);
            await lobbyHandler.QueryTimer();

            await Task.Delay(-1);
        }

        public static async Task Log(string message)
        {
            await Log(new LogMessage(LogSeverity.Info, string.Empty, message));
        }
        /// <summary>
        /// Logs a message. Use this over <see cref="Console.WriteLine()"/> when possible.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public static Task Log(LogMessage message)
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
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        public async Task UpdateLobbyInformation()
        {
            //VTOL lobbies
            if (config.vtolLobbyChannelId == 0)
            {
                await Log(new LogMessage(LogSeverity.Warning, "VTOL Embed Builder", "VTOL Lobby Channel ID is not set!"));
            }
            else
            {
                EmbedBuilder vtolEmbedBuilder = new();
                vtolEmbedBuilder.WithColor(Discord.Color.DarkGrey).WithCurrentTimestamp().WithTitle("VTOL VR Lobbies:");
                if (lobbyHandler.vtolLobbies.Count > 0)
                {
                    foreach (VTOLLobby lobby in lobbyHandler.vtolLobbies)
                    {
                        if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty || lobby.ScenarioText == string.Empty)
                        {
                            await Log(new LogMessage(LogSeverity.Warning, "VTOL Embed Builder", "Invalid lobby state!"));
                            continue;
                        }
                        string content = $"{lobby.ScenarioText}\n{lobby.MemberCount} Players";
                        vtolEmbedBuilder.AddField(lobby.LobbyName, content);
                    }
                }
                else vtolEmbedBuilder.AddField("No lobbies!", "Check back later!");

                var vtolChannel = (ISocketMessageChannel)await client.GetChannelAsync(config.vtolLobbyChannelId);

                if(vtolLobbyMessageId != 0 && await vtolChannel.GetMessageAsync(vtolLobbyMessageId) != null)
                {
                    await vtolChannel.ModifyMessageAsync(vtolLobbyMessageId, m => m.Embed = vtolEmbedBuilder.Build());
                }
                else
                {
                    var newMessage = await vtolChannel.SendMessageAsync(embed: vtolEmbedBuilder.Build());
                    vtolLobbyMessageId = newMessage.Id;
                }
                
            }

            //JBR lobbies
            if (config.jetborneLobbyChannelId == 0)
            {
                await Log(new LogMessage(LogSeverity.Warning, "JBR Embed Builder", "JBR Lobby Channel ID is not set!"));
            }
            else
            {
                EmbedBuilder jetborneEmbedBuilder = new();
                jetborneEmbedBuilder.WithColor(Discord.Color.DarkGrey).WithCurrentTimestamp().WithTitle("Jetborne Racing Lobbies:");
                if (lobbyHandler.jetborneLobbies.Count > 0)
                {
                    foreach (JetborneLobby lobby in lobbyHandler.jetborneLobbies)
                    {
                        if (lobby.OwnerName == string.Empty || lobby.LobbyName == string.Empty)
                        {
                            await Log(new LogMessage(LogSeverity.Warning, "JBR Embed Builder", "Invalid lobby state!"));
                            continue;
                        }
                        string content = $"{lobby.MemberCount} Players\nLap {lobby.CurrentLap}/{lobby.RaceLaps}";
                        jetborneEmbedBuilder.AddField(lobby.LobbyName, content);
                    }
                }
                else jetborneEmbedBuilder.AddField("No lobbies!", "Check back later!");

                var jetborneChannel = (ISocketMessageChannel)await client.GetChannelAsync(config.jetborneLobbyChannelId);

                if (jetborneLobbyMessageId != 0 && await jetborneChannel.GetMessageAsync(jetborneLobbyMessageId) != null)
                {
                    await jetborneChannel.ModifyMessageAsync(jetborneLobbyMessageId, m => m.Embed = jetborneEmbedBuilder.Build());
                }
                else
                {
                    var newMessage = await jetborneChannel.SendMessageAsync(embed: jetborneEmbedBuilder.Build());
                    jetborneLobbyMessageId = newMessage.Id;
                }
            }
        }

        //Event methods vvv

        public async Task ClientReady()
        {
            commandHandler = new();
            commandBuilder = new(client);
            client.InteractionCreated += commandHandler.ClientInteractionCreated;
            await commandBuilder.BuildCommands();
        }
        
        private static void OnExit(object sender, EventArgs e)
        {
            if (forceDontSaveConfig) return;
            Console.WriteLine("------");
            if (config.shouldSave)
            {
                Console.WriteLine("Saving config!");
                config.Save(false);
                Console.WriteLine("Goodbye!");
            }
            else
                Console.WriteLine("Not saving config! Goodbye!");
        }
    }
}
