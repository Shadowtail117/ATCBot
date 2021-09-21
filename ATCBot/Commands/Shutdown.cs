using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    class Shutdown : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name { get; set; } = "shutdown";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("shutdown")
            .WithDescription("Shuts down the bot. Requires Manage Server permissions. WARNING: Requires manual restart!");

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="command"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                Program.shouldShutdown = true;
                return "Shutting down! o7";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
