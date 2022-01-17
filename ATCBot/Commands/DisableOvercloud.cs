using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class DisableOvercloud : Command
    {
        public override string Name { get; set; } = "disableovercloud";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("disableovercloud")
            .WithDescription("Output a message telling you how to disable Overcloud.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "To disable Overcloud, follow these steps:" +
                "\n1. Press and hold the Windows Key and the R key. A menu should pop up in the bottom left of your screen." +
                "\n2. In this menu, paste the following: " + @"`%appdata%\..\Roaming\Boundless Dynamics, LLC\VTOLVR\SaveData`" +
                "\n3. A directory should appear. Inside should be a file named \"gameSettings.cfg\"." +
                "\n4. Open the file, and look for a line that says `USE_OVERCLOUD = True` and delete it. If this line isn't present, overcloud isn't on. Please note that overcloud isn't supported in the current version of VTOL VR.";
        }
    }
}
