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
            .WithDescription("Force a manual update to the lobby messages. Requires permission.");

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                Program.lobbyHandler.ResetQueryTimer();
                return "Initiated a manual update!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
