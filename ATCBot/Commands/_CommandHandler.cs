using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    /// <summary>
    /// Class to handle invocation of slash commands.
    /// </summary>
    public class CommandHandler
    {
        /// <summary>
        /// Called whenever a slash command is invoked to the bot.
        /// </summary>
        /// <param name="arg">The context of the command.</param>
        /// <exception cref="Exception">Thrown if an interaction other than a slash command is registered, as that shouldn't be possible.</exception>
        public async Task ClientInteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                string argsAsText = "";
                if (!(command.Data.Options == null))
                {
                    foreach (var option in command.Data.Options) argsAsText += option.Value.ToString() + ", ";
                    argsAsText = argsAsText[0..^2];
                }

                Program.LogInfo($"Received slash command \"{command.Data.Name}\"" +
                    argsAsText == "" ? "" : $" with arguments \"{argsAsText}\"", $"{command.User.Username} in {command.Channel.Name}");
                Command c = Command.AllCommands.Find(c => c.Name.Equals(command.Data.Name));
                await command.RespondAsync(c.Action(command));
            }
            else throw new Exception("Impossible client interaction!");
            if(Program.shouldShutdown)
            {
                Environment.Exit(0);
            }
        }
    }
}
