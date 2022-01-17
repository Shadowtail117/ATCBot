using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class LobbyMissing : Command
    {
        public override string Name { get; set; } = "lobbymissing";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("lobbymissing")
            .WithDescription("Get info on why you can't see someone's lobby.");

        public override string Action(SocketSlashCommand command)
        {
            return
                "Currently, modded and unmodded people aren't able to play together, so make sure you are on the same version as your friend. " +
                "\nIf you are on the same version, due to the way the SteamAPI works, sometimes people in different regions will not be able to see each other's lobbies. While the dev has tried to get around this issue, from time to time it'll crop up again.";
        }
    }
}
