using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// Command to start updating the lobby information.
    /// </summary>
    public class StartUpdating : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name { get; set; } = "startupdating";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("startupdating")
            .WithDescription("Start updating the lobby data. Requires permission.");

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="command"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
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
