using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ATCBot.Structs;
using Discord;
using Discord.Net;
using Discord.WebSocket;

using Newtonsoft.Json;
using Steamworks;
using Steamworks.Data;

namespace ATCBot
{
    partial class Program
    {
        public static List<VTOLLobby> LastVTOLLobbies = new ();
        public static List<JetborneLobby> LastJetborneLobbies = new ();
        
        private const int _vtolID = 667970;
        private const int _jetborneID = 1397650;

        public DiscordSocketClient client;

        public static Config config = new Config();
        private static bool shouldSaveConfig = true;

        public  bool shouldUpdate = false;
        
        /// <summary>
        /// How often to check the lobbies
        /// </summary>
        private TimeSpan _queryDelay = TimeSpan.FromMinutes(0.5f);

        static void Main(string[] args)
        {
            //Stuff to set up the console
            Console.Title = "ATCBot v." + Config.version;
            Console.WriteLine($"Booting up ATCBot version {Config.version}.");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            if (!config.Load(out config))
            {
                shouldSaveConfig = false;
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
            client.InteractionCreated += ClientInteractionCreated;
            client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();
            await client.SetGameAsync(config.prefix + "commands");

            await QueryTimer();
            
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (!message.Content.Equals("!test"))
                return;
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"VTOL VR Lobbies | Count = {LastVTOLLobbies.Count}");
            foreach (VTOLLobby lobby in LastVTOLLobbies)
            {
                builder.AppendLine($"{lobby.LobbyName} | " +
                                   $"{lobby.OwnerName} | " +
                                   $"{lobby.ScenarioText} | " +
                                   $"Players = {lobby.MemberCount}");
                
            }

            builder.AppendLine();
            builder.AppendLine($"Jetborne Racing Lobbies | Count = {LastJetborneLobbies.Count}");
            foreach (JetborneLobby lobby in LastJetborneLobbies)
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
            await BuildCommands();
        }

        private async Task QueryTimer()
        {
            await GetData();
            await Task.Delay(_queryDelay);
            await QueryTimer();
        }

        private async Task GetData()
        {
            await Log($"Getting Lobbies at {DateTime.Now}");

            SteamClient.Init(_vtolID);
            LastVTOLLobbies.Clear();
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.RequestAsync();
            
            foreach (Lobby lobby in lobbies)
            {
                LastVTOLLobbies.Add(new VTOLLobby(lobby));
            }

            await ShutdownSteam();
            
            SteamClient.Init(_jetborneID);
            LastJetborneLobbies.Clear();
            lobbies = await SteamMatchmaking.LobbyList.RequestAsync();
            
            foreach (Lobby lobby in lobbies)
            {
                LastJetborneLobbies.Add(new JetborneLobby(lobby));
            }

            await ShutdownSteam();
        }

        private async Task ShutdownSteam()
        {
            // These delays are needed because an error happens if 
            // init and shutdown are ran at the same time
            await Task.Delay(TimeSpan.FromSeconds(1));
            SteamClient.Shutdown();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        static async Task Log(string message)
        {
            await Log(new LogMessage(LogSeverity.Info, string.Empty, message));
        }
        /// <summary>
        /// Logs a message. Use this over <see cref="Console.WriteLine()"/> when possible.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        internal static Task Log(LogMessage message)
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
            if (!shouldSaveConfig) return;
            Console.WriteLine("\nShutting down!");
            config.Save();
        }
    }
}
