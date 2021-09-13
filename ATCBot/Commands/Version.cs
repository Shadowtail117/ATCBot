using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    public class Version : Command
    {
        public override string Name { get; set; } = "version";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("version")
            .WithDescription("Get the current version of the bot.");
        public override string Action(SocketSlashCommand command)
        {
            return "ATCBot is running version " + Config.version;
        }
    }
}