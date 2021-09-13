using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class SetConfig : Command
    {
        public override string Name { get; set; } = "setconfig";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
        .WithName("setconfig")
        .WithDescription("set the value of a configuration option.")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("config")
            .WithDescription("The config item.")
            .WithRequired(true)
            .AddChoice("delay", 1)
            .WithType(ApplicationCommandOptionType.Integer)
        ).AddOption("value", ApplicationCommandOptionType.String, "The value to set the config item to.");

        public override string Action(SocketSlashCommand command)
        {
            if (command.User is SocketGuildUser u)
            {
                if (u.GuildPermissions.ManageGuild == true)
                {
                    switch(Convert.ToInt32(command.Data.Options.First().Value))
                    {
                        case 1:
                            bool successful = int.TryParse((string)command.Data.Options.ElementAt(1).Value, out int value);
                            if(successful)
                            {
                                Program.config.delay = value;
                                return $"Successfully set delay to {value} seconds!";
                            }
                            else
                            {
                                return $"\"Sorry, I couldn't translate your input into an integer.\"";
                            }
                        default: throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\"");
                    }
                }
                else return "Sorry, you don't have enough permissions for this!";
            }
            else throw new System.Exception("Could not get user permissions!");
        }
    }
}
