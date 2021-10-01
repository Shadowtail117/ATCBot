using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Collections.Generic;

namespace ATCBot.Commands
{
    /// <summary>
    /// Base class for slash commands to inherit from.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// A list of all commands found through using Reflection.
        /// </summary>
        public static List<Command> AllCommands { get; set; } = new();

        private static bool warnedBotRoleNotSet;

        internal static bool HasPerms(SocketGuildUser u)
        {
            if (Program.config.botRoleId == 0)
            {
                if (!warnedBotRoleNotSet)
                {
                    Log.LogDebug("Bot role ID not set, defaulting to Manage Server for permission checking...", announce: true);
                    warnedBotRoleNotSet = true;
                }

                return u.GuildPermissions.ManageGuild;
            }
            else return u.GuildPermissions.Administrator || u.Roles.FirstOrDefault(t => t.Id == Program.config.botRoleId) != null;
        }
        internal static bool HasPerms(SocketUser u)
        {
            if (u is SocketGuildUser g)
            {
                return HasPerms(g);
            }
            else return false;
        }

        internal static bool IsOwner(SocketGuildUser u) => u.Id == Program.config.botOwnerId;
        internal static bool IsOwner(SocketUser u)
        {
            if (u is SocketGuildUser g)
            {
                return HasPerms(g);
            }
            else return false;
        }

        /// <summary>
        /// The name of the command.
        /// </summary>
        /// <remarks>MUST match the builder's name.</remarks>
        public abstract string Name { get; set; }

        /// <summary>
        /// The slash command builder used to create the command. Must be initialized at declaration.
        /// </summary>
        public abstract SlashCommandBuilder Builder { get; set; }

        /// <summary>
        /// The action to take when this command is invoked.
        /// </summary>
        /// <param name="command">The context of the command when invoked.</param>
        /// <returns>A string to output back to the user who invoked the command.</returns>
        public abstract string Action(SocketSlashCommand command);
    }
}
