using Discord;
using Discord.WebSocket;

using System;
using System.Linq;

namespace ATCBot.Commands
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    class GetConfig : Command
    {
        public override string Name { get; set; } = "getconfig";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
        .WithName("getconfig")
        .WithDescription("Get the value of a configuration option. Requires permission.")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("config")
            .WithDescription("The config item.")
            .WithRequired(true)
            .AddChoice("delay", 1)
            .AddChoice("updating", 2)
            .AddChoice("vtolchannelid", 3)
            .AddChoice("jetbornechannelid", 4)
            .AddChoice("systemmessageid", 5)
            .AddChoice("botroleid", 6)
            .WithType(ApplicationCommandOptionType.Integer)
        );

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                return Convert.ToInt32(command.Data.Options.First().Value) switch
                {
                    1 => Program.config.delay.ToString() + " seconds",
                    2 => Program.updating.ToString(),
                    3 => Program.config.vtolLobbyChannelId.ToString(),
                    4 => Program.config.jetborneLobbyChannelId.ToString(),
                    5 => Program.config.systemMessageChannelId.ToString(),
                    6 => Program.config.botRoleId.ToString(),
                    _ => throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\""),
                };
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
