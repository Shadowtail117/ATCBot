using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class StopUpdating : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name { get; set; } = "stopupdating";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("stopupdating")
            .WithDescription("Stop updating the lobby data. Requires permission.");

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="command"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
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
