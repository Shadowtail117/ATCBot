using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class Shutdown : Command
    {
        public override string Name { get; set; } = "shutdown";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("shutdown")
            .WithDescription("Shuts down the bot. Requires Manage Server permissions. WARNING: Requires manual restart!");

        public override string Action(SocketSlashCommand command)
        {
            Program.shouldShutdown = true;
            return "Shutting down! o7";
        }
    }
}
