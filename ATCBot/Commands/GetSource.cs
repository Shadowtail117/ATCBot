using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class GetSource : Command
    {
        public override string Name { get; set; } = "getsource";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("getsource")
            .WithDescription("Get my source code.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "You can view my source code here https://github.com/Shadowtail117/ATCBot/tree/dev.";
        }
    }
}
