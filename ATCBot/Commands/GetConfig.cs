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
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name { get; set; } = "getconfig";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="command"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
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
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
