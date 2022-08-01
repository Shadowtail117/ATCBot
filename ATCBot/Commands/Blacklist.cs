using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    internal class Blacklist : Command
    {
        public override string Name { get; set; } = "blacklist";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("blacklist")
            .WithDescription("Blacklist a steamID from appearing in lobbies.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("id")
                .WithDescription("The steamID to blacklist.")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String)
            );

        public override bool Ephemeral { get; set; } = true;

        public override string Action(SocketSlashCommand command)
        {
            if(HasPerms(command.User))
            {
                bool successful = long.TryParse((string) command.Data.Options.First().Value, out long id);
                if(!successful)
                {
                    Log.LogWarning($"Could not parse blacklist ID \"{(string)command.Data.Options.First().Value}\"");
                    return "Invalid ID, please double check that you've input a number.";
                }
                ATCBot.Blacklist.blacklist.Add(id);
                ATCBot.Blacklist.Save();
                return $"Successfully added `{id}` to the blacklist.";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
