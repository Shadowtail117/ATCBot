using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    public class StopUpdating : Command
    {
        public override string Name { get; set; } = "stopupdating";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("stopupdating")
            .WithDescription("Stop updating the lobby data. Requires Manage Server permissions.");

        public override string Action(SocketSlashCommand command)
        {
            if (command.User is SocketGuildUser u)
            {
                if (u.GuildPermissions.ManageGuild == true)
                {
                    if (!Program.shouldUpdate)
                        return "Already not updating!";
                    else
                    {
                        Program.shouldUpdate = false;
                        return "Stopped updating!";
                    }
                }
                else return "Sorry, you don't have enough permissions for this!";
            }
            else throw new System.Exception("Could not get user permissions!");
        }
    }
}
