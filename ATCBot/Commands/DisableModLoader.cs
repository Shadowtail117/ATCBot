using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class DisableModLoader : Command
    {
        public override string Name { get; set; } = "disablemodloader";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("disablemodloader")
            .WithDescription("Get info on how to disable your Mod Loader.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "Here's a video showing you how to disable your mod loader https://youtu.be/CQpn1gc2gL4."; // no this is not a rick roll (it's a rick roll)
        }
    }
}
