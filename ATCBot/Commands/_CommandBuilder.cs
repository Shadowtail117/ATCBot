using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System;

using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// Class for building all slash commands.
    /// </summary>
    public class CommandBuilder
    {
        private DiscordSocketClient client;

        private Config config = Program.config;

        /// <summary>
        /// Fetches all commands using Reflection, builds them, and overwrites any previous commands with the new ones.
        /// </summary>
        /// <remarks>Called when the bot deems itself ready for work.</remarks>
        public async Task BuildCommands()
        {
            //Get all commands (all classes that inherit from Command)
            var commands = Assembly.GetAssembly(typeof(Command)).GetTypes().Where(t => t.IsSubclassOf(typeof(Command)));
            foreach(var c in commands)
            {
                var command = (Command)Activator.CreateInstance(c);
                
                Command.AllCommands.Add(command);
            }

            if (config.shouldBuildCommands)
            {
                Program.LogDebug("Building slash commands...", "Slash Command Builder");
                config.shouldBuildCommands = false;
            }
            else
            {
                Program.LogDebug("Skipping building slash commands...", "Slash Command Builder");
                return;
            }

            try
            {
                var commandList = new List<SlashCommandProperties>();
                foreach (var c in Command.AllCommands)
                    commandList.Add(c.Builder.Build());
                await client.BulkOverwriteGlobalApplicationCommandsAsync(commandList.ToArray());
            }
            catch (ApplicationCommandException e)
            {
                Program.LogCritical("Could not initialize a slash command!", e, "Slash Command Builder");
                throw;
            }
            config.shouldBuildCommands = false;
        }

        /// <summary>
        /// Default constructor. Assign the current bot client.
        /// </summary>
        /// <param name="client">The current client to write the new commands to.</param>
        public CommandBuilder(DiscordSocketClient client)
        {
            this.client = client;
        }
    }
}
