using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class LogHelp : Command
    {
        public override string Name { get; set; } = "loghelp";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("loghelp")
            .WithDescription("Get a shortcut to getting your player.log.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "To get your player.log, follow these steps:" +
                "\n1. Press and hold the Windows Key and the R key. A menu should pop up in the bottom left of your screen." +
                "\n2. In this menu, paste the following: " + @"`%appdata%\..\LocalLow\Boundless Dynamics, LLC\VTOLVR`" +
                "\n3. A directory should appear. Inside should be a file named \"Player.log\"." +
                "\n4. Upload this file into <#444150937079250945>, or whatever channel you are asked to do so in.";
        }
    }
}
