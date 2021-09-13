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
    partial class Program
    {
        public const int vtolID = 667970;
        public const int jetborneID = 1397650;

        public DiscordSocketClient client;

        public CommandBuilder commandBuilder;

        public CommandHandler commandHandler;

        public LobbyHandler lobbyHandler;

        private static bool forceDontSaveConfig = true;

        public static bool shouldUpdate = false;

        public static Config config = Config.config;

        static void Main(string[] args)
        {
            //Stuff to set up the console
            Console.Title = "ATCBot v." + Config.version;
            Console.WriteLine($"Booting up ATCBot version {Config.version}.");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

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
            client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();
            await client.SetGameAsync(config.prefix + "commands");

            lobbyHandler = new();
            await lobbyHandler.QueryTimer();

            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (!message.Content.Equals("!test"))
                return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"VTOL VR Lobbies | Count = {lobbyHandler.vtolLobbies.Count}");
            foreach (VTOLLobby lobby in lobbyHandler.vtolLobbies)
            {
                builder.AppendLine($"{lobby.LobbyName} | " +
                                   $"{lobby.OwnerName} | " +
                                   $"{lobby.ScenarioText} | " +
                                   $"Players = {lobby.MemberCount}");

            }

            builder.AppendLine();
            builder.AppendLine($"Jetborne Racing Lobbies | Count = {lobbyHandler.jetborneLobbies.Count}");
            foreach (JetborneLobby lobby in lobbyHandler.jetborneLobbies)
            {
                builder.AppendLine($"{lobby.LobbyName} | " +
                                   $"{lobby.OwnerName} | " +
                                   $"{lobby.MemberCount} | " +
                                   $"{lobby.CurrentLap}/{lobby.RaceLaps}");
            }
            await message.Channel.SendMessageAsync(builder.ToString());
        }

        public async Task ClientReady()
        {
            commandHandler = new();
            commandBuilder = new(client);
            client.InteractionCreated += commandHandler.ClientInteractionCreated;
            await commandBuilder.BuildCommands();
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

        private static void OnExit(object sender, EventArgs e)
        {
            if (forceDontSaveConfig) return;
            Console.WriteLine("----------");
            Console.WriteLine("Would you like to save the current configuration to disk? (y/n)");
            if (char.ToLower(Console.ReadKey().KeyChar) == 'y')
            {
                Console.WriteLine("Saving! Press any key to exit.");
                config.Save();
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine("Not saving! Press any key to exit.");
                Console.ReadLine();
                return;
            }
        }
    }
}
