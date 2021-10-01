using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class SetLogVerbosity : Command
    {
        public override string Name { get; set; } = "setlogverbosity";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("setlogverbosity")
            .WithDescription("Set verbosity of the logs. Requires being the bot's owner.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("verbosity")
                .WithDescription("The verbosity to set.")
                .AddChoice("regular", 0)
                .AddChoice("verbose", 1)
                .AddChoice("debug", 2)
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer));

        public override string Action(SocketSlashCommand command)
        {
            if (Program.config.botOwnerId == 0)
            {
                return "Owner ID has not been set in the config! Please set that first.";
            }

            if (IsOwner(command.User))
            {
                Program.config.logVerbosity = (Log.LogVerbosity)Convert.ToInt32(command.Data.Options.ElementAt(0).Value);
                return $"Successfully set log verbosity to {Program.config.logVerbosity}!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
