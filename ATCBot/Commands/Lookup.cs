using ATCBot.Structs;

using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    internal class Lookup : Command
    {
        public override string Name { get; set; } = "lookup";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("lookup")
            .WithDescription("Look up the SteamID of the owner of a lobby. Requires permission.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("name")
                .WithDescription("The exact name of the host of the lobby.")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
            );

        public override string Action(SocketSlashCommand command)
        {
            if(HasPerms(command.User)) {
                VTOLLobby? lobby = null;
                string nameToSearch = (string) command.Data.Options.First().Value;
                foreach(VTOLLobby l in Program.lobbyHandler.vtolLobbies)
                {
                    if(l.OwnerName.Equals(nameToSearch))
                    {
                        lobby = l;
                        Log.LogInfo($"Found lobby \"{l.LobbyName}\" with owner \"{l.OwnerName}\" on lookup.");
                        break;
                    }
                }
                return lobby == null ? "Could not find that lobby." : $"SteamID of lobby \"{lobby.Value.LobbyName}\" is `{lobby.Value.OwnerId}`.";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
