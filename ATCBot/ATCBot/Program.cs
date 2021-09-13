using System;
using System.Threading.Tasks;

using Discord;
using Discord.Net;
using Discord.WebSocket;

using Newtonsoft.Json;

using ATCBot.Commands;
using System.Collections.Generic;

namespace ATCBot
{
    partial class Program
    {
        public DiscordSocketClient client;

        public CommandBuilder commandBuilder;

        public CommandHandler commandHandler;

        private static bool shouldSaveConfig = true;

        public static Config config = Config.config;

        public static bool shouldUpdate = false;

        static void Main(string[] args)
        {
            //Stuff to set up the console
            Console.Title = "ATCBot v." + Config.version;
            Console.WriteLine($"Booting up ATCBot version {Config.version}.");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            config = new Config();

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
            //client.MessageReceived += MessageReceived;

            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();
            await client.SetGameAsync(config.prefix + "commands");

            await Task.Delay(-1);
        }

        public async Task ClientReady()
        {
            commandHandler = new();
            commandBuilder = new(client);
            client.InteractionCreated += commandHandler.ClientInteractionCreated;
            await commandBuilder.BuildCommands();
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
            if (!shouldSaveConfig) return;
            Console.WriteLine("\nShutting down!");
            config.Save();
        }
    }
}
