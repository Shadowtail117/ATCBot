using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace ATCBot.Commands
{
    public class CommandHandler
    {
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
