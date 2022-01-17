using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class VerifyHelp : Command
    {
        public override string Name { get; set; } = "verifyhelp";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("verifyhelp")
            .WithDescription("Get info on how to verify your game files.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "To verify your game files, follow these steps:" +
                "\n1. Navigate to VTOL VR in your steam library, and right click on it." +
                "\n2. Click properties." +
                "\n3. Click local files." +
                "\n4. Click verify integrity of game files.";
        }
    }
}
