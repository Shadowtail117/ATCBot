using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    internal class StartUpdating : Command
    {
        public override string Name { get; set; } = "startupdating";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("startupdating")
            .WithDescription("Start updating the lobby data. Requires permission.");

        public override bool Ephemeral { get; set; } = true;

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                if (Program.updating)
                    return "Already updating!";
                else
                {
                    Program.updating = true;
                    return "Started updating!";
                }
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
