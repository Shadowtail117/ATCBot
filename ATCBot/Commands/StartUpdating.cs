using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    public class StartUpdating : Command
    {
        public override string Name { get; set; } = "startupdating";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("startupdating")
            .WithDescription("Start updating the lobby data. Requires Manage Server permissions.");

        public override string Action(SocketSlashCommand command)
        {
            if (command.User is SocketGuildUser u)
            {
                if (u.GuildPermissions.ManageGuild == true)
                {
                    if (Program.shouldUpdate)
                        return "Already updating!";
                    else
                    {
                        Program.shouldUpdate = true;
                        return "Started updating!";
                    }
                }
                else return "Sorry, you don't have enough permissions for this!";
            }
            else throw new System.Exception("Could not get user permissions!");
        }
    }
}
