using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;

namespace ATCBot.Commands
{
    public abstract class Command
    {
        public static List<Command> AllCommands { get; set; } = new();

        public abstract string Name { get; set; }

        public abstract SlashCommandBuilder Builder { get; set; }

        public abstract string Action(SocketSlashCommand command);
    }
}
