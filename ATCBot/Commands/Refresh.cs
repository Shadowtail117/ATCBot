using Discord;
using Discord.WebSocket;

using System;

namespace ATCBot.Commands
{
    class Refresh : Command
    {
        public override string Name { get; set; } = "refresh";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("refresh")
            .WithDescription("Force the bot to delete previous messages. Requires Manage Server permissions.");

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                Program.shouldRefresh = true;
                return "Will refresh on the next update!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
