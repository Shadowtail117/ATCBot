using Discord;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    class RebuildCommands : Command
    {
        public override string Name { get; set; } = "rebuildcommands";

        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("rebuildcommands")
            .WithDescription("Rebuild commands on next restart. Requires being the bot's owner.");

        public override string Action(SocketSlashCommand command)
        {
            if (Program.config.botOwnerId == 0)
            {
                return "Owner ID has not been set in the config! Please set that first.";
            }

            if (IsOwner(command.User))
            {
                Program.config.shouldBuildCommands = !Program.config.shouldBuildCommands;

                return $"Will{(Program.config.shouldBuildCommands ? " " : " not ")}rebuild commands on next start!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
