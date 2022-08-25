using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    internal class StopUpdating : Command
    {
        public override string Name { get; set; } = "stopupdating";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("stopupdating")
            .WithDescription("Stop updating the lobby data. Requires permission.");

        public override bool Ephemeral { get; set; } = true;

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                if (!Program.updating)
                    return "Already not updating!";
                else
                {
                    Program.updating = false;
                    return "Stopped updating!";
                }
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
