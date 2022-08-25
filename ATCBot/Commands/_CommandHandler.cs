﻿using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

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
                if (!(command.Data.Options == null) && command.Data.Options.Count > 0)
                {
                    foreach (var option in command.Data.Options) argsAsText += option.Value.ToString() + ", ";
                    argsAsText = argsAsText[0..^2];
                }

                string logMessage = $"Received slash command \"{command.Data.Name}\"{(argsAsText == "" ? "" : $" with parameters \"{argsAsText}\"")}.";
                string logSource = $"{command.User.Username}#{command.User.Discriminator} in {command.Channel.Name}";
                Log.LogInfo(logMessage, logSource, true);
                Command c = Command.AllCommands.Find(c => c.Name.Equals(command.Data.Name));
                await command.RespondAsync(c.Action(command), ephemeral: c.Ephemeral, options: Program.requestOptions);
            }
            else throw new Exception("Impossible client interaction!");
        }
    }
}
