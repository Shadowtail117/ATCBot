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
    public class CommandBuilder
    {
        public DiscordSocketClient client;
        public Config config = Program.config;

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
                await Program.Log(new LogMessage(LogSeverity.Debug, "Slash Command Builder", "Building slash commands..."));
                config.shouldBuildCommands = false;
            }
            else
            {
                await Program.Log(new LogMessage(LogSeverity.Debug, "Slash Command Builder", "Skipping building slash commands..."));
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
                await Program.Log(new LogMessage(LogSeverity.Critical, "Slash Command Builder", "Could not initialize a slash command!", e));
                throw;
            }
            config.shouldBuildCommands = false;
        }

        public CommandBuilder(DiscordSocketClient client)
        {
            this.client = client;
        }
    }
}
