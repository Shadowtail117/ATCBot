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
            .AddChoice("vtolchannelid", 3)
            .AddChoice("jetbornechannelid", 4)
            .WithType(ApplicationCommandOptionType.Integer)
        );                

        public override string Action(SocketSlashCommand command)
        {
            if (command.User is SocketGuildUser u)
            {
                if (u.GuildPermissions.ManageGuild == true)
                {
                    return Convert.ToInt32(command.Data.Options.First().Value) switch
                    {
                        1 => Program.config.delay.ToString(),
                        2 => Program.shouldUpdate.ToString(),
                        3 => Program.config.vtolLobbyChannelId.ToString(),
                        4 => Program.config.jetborneLobbyChannelId.ToString(),
                        _ => throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\""),
                    };
                }
                else return "Sorry, you don't have enough permissions for this!";
            }
            else throw new Exception("Could not get user permissions!");
        }
    }
}
