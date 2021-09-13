using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class GetConfig : Command
    {
        public override string Name { get; set; } = "getconfig";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
        .WithName("getconfig")
        .WithDescription("Get the value of a configuration option.")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("config")
            .WithDescription("The config item.")
            .WithRequired(true)
            .AddChoice("delay", 1)
            .AddChoice("updating", 2)
            .WithType(ApplicationCommandOptionType.Integer)
        );                

        public override string Action(SocketSlashCommand command)
        {
            if (command.User is SocketGuildUser u)
            {
                if (u.GuildPermissions.ManageGuild == true)
                {
                    switch(Convert.ToInt32(command.Data.Options.First().Value))
                    {
                        case 1:
                            return Program.config.delay.ToString();
                        case 2:
                            return Program.shouldUpdate.ToString();
                        default: throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\"");
                    }
                }
                else return "Sorry, you don't have enough permissions for this!";
            }
            else throw new Exception("Could not get user permissions!");
        }
    }
}
