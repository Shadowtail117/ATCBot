using System;
using System.Collections.Generic;
using System.Threading.Tasks;


using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace ATCBot
{
    partial class Program
    {
        public async Task ClientInteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                await Log(new LogMessage(LogSeverity.Info, command.Channel.Name, "Received slash command \"" + command.Data.Name + "\"."));
                switch (command.Data.Name)
                {
                    case "start":
                        await command.RespondAsync(StartUpdating());
                        break;
                    case "stop":
                        await command.RespondAsync(StopUpdating());
                        break;
                    case "version":
                        await command.RespondAsync(Version());
                        break;
                }
            }
            else throw new Exception("Impossible client interaction!");
        }

        internal string StartUpdating()
        {
            if (shouldUpdate)
                return "Already updating!";
            shouldUpdate = true;
            return "Starting updating!";

        }
        internal string StopUpdating()
        {
            if (!shouldUpdate)
                return "Already not updating!";
            shouldUpdate = false;
            return "Stopped updating!";
        }
        internal string Version()
        {
            return "Current version is: " + Config.version;
        }
    }
}
