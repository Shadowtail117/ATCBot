using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.Net;

namespace ATCBot
{
    partial class Program
    {
        public async Task BuildCommands()
        {
            if(config.shouldBuildCommands)
            {
                await Log(new LogMessage(LogSeverity.Debug, "Slash Command Builder", "Building slash commands..."));
                config.shouldBuildCommands = false;
            }
            else
            {
                await Log(new LogMessage(LogSeverity.Debug, "Slash Command Builder", "Skipping building slash commands..."));
                return;
            }

            var commands = new List<SlashCommandBuilder>();

            StartProcessing();
            StopProcessing();
            Version();

            try
            {
                foreach(var command in commands)
                    await client.CreateGlobalApplicationCommandAsync(command.Build());
            }
            catch (ApplicationCommandException e)
            {
                await Log(new LogMessage(LogSeverity.Critical, "Slash Command Builder", "Could not initialize a slash command!", e));
                throw;
            }

            void StartProcessing()
            {
                var startCommand = new SlashCommandBuilder();
                startCommand.WithName("start");
                startCommand.WithDescription("Start updating the status channel. Admins only.");
                commands.Add(startCommand);
            }

            void StopProcessing()
            {
                var stopCommand = new SlashCommandBuilder();
                stopCommand.WithName("stop");
                stopCommand.WithDescription("Stop updating the status channel. Admins only.");
                commands.Add(stopCommand);
            }

            void Version()
            {
                var versionCommand = new SlashCommandBuilder();
                versionCommand.WithName("version");
                versionCommand.WithDescription("Get the version of the bot!");
                commands.Add(versionCommand);
            }
        }
    }
}
