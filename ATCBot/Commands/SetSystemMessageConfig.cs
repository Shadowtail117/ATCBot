using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using options = ATCBot.Config.SystemMessageConfigOptions;

namespace ATCBot.Commands
{
    internal class SetSystemMessageConfig : Command
    {
        public override string Name { get; set; } = "setsystemmessageconfig";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("setsystemmessageconfig")
            .WithDescription("Change what system messages should be sent. Requires permission.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("type")
                .WithDescription("The type of message to edit.")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer)
                .AddChoice("ConnectionStatus", 0)
                .AddChoice("Queries", 1)
                .AddChoice("CommandReceived", 2)
                .AddChoice("WatchdogWarnings", 3)
                .AddChoice("Info", 4)
                .AddChoice("Warning", 5)
                .AddChoice("Error", 6)
                .AddChoice("Verbose", 7)
                .AddChoice("Debug", 8))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("value")
                .WithDescription("What to set this type to.")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer)
                .AddChoice("Enabled", 1)
                .AddChoice("Disabled", 0));

        public override string Action(SocketSlashCommand command)
        {
            if(HasPerms(command.User))
            {
                int type = Convert.ToInt32(command.Data.Options.First().Value);
                options option = type switch
                {
                    0 => options.ConnectionStatus,
                    1 => options.Queries,
                    2 => options.CommandReceived,
                    3 => options.WatchdogWarnings,
                    4 => options.Info,
                    5 => options.Warning,
                    6 => options.Error,
                    7 => options.Verbose,
                    8 => options.Debug,
                    _ => throw new Exception("Impossible option!")
                };
                bool setting = Convert.ToInt32(command.Data.Options.ElementAt(1).Value) == 1 ? true : false;
                Program.config.systemMessagesConfig.Value[option] = setting;
                return $"Successfully {(setting ? "enabled" : "disabled")} \"{option}\" option!";
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}
