using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class SetStatusMessage : Command
    {
        public override string Name { get; set; } = "setstatusmessage";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("setstatusmessage")
            .WithDescription("Sets the status of the bot as well as a custom message. Requires permission.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("status")
                .WithDescription("The status of the bot to set.")
                .WithRequired(true)
                .AddChoice("Online", 0)
                .AddChoice("Offline", 1)
                .AddChoice("Custom", 2)
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("custommessage")
                .WithDescription("A custom message to set, if you chose \"Custom\" for the first argument.")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(false)
            );

        public override string Action(SocketSlashCommand command)
        {
            string output;
            if (HasPerms(command.User))
            {
                switch (Convert.ToInt32(command.Data.Options.First().Value))
                {
                    case 0:
                        Program.SetStatus(Program.Status.Online);
                        break;
                    case 1:
                        Program.SetStatus(Program.Status.Offline);
                        break;
                    case 2:
                        Program.SetStatus(Program.Status.Custom);
                        break;
                }
                output = "Successfully set status to " + Program.config.status;

                if(command.Data.Options.Count > 1 && command.Data.Options.ElementAt(1).Value != null)
                {
                    string custom = Convert.ToString(command.Data.Options.ElementAt(1).Value);
                    Program.config.customStatusMessage = custom;
                    output += $" and custom status message to \"{custom}\"";
                }
                _ = Program.UpdateStatusMessage();
                return output + "!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
