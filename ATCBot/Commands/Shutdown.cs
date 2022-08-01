using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    internal class Shutdown : Command
    {
        public override string Name { get; set; } = "shutdown";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("shutdown")
            .WithDescription("Shuts down the bot. Requires permission. WARNING: Requires manual restart!");

        public override bool Ephemeral { get; set; } = false;

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
