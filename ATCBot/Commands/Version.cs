using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class Version : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name { get; set; } = "version";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("version")
            .WithDescription("Get the current version of the bot.");

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="command"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override string Action(SocketSlashCommand command)
        {
            return "ATCBot is running version " + Config.version;
        }
    }
}