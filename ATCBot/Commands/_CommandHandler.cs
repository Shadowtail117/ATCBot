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
                await Program.Log(new LogMessage(LogSeverity.Info, command.Channel.Name, "Received slash command \"" + command.Data.Name + "\"."));
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
