﻿using Discord;
using Discord.WebSocket;

using System;
using System.Linq;

namespace ATCBot.Commands
{
    class SetConfig : Command
    {
        public override string Name { get; set; } = "setconfig";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
        .WithName("setconfig")
        .WithDescription("Set the value of a configuration option. Requires permission.")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("config")
            .WithDescription("The config item.")
            .WithRequired(true)
            .AddChoice("delay", 1)
            .AddChoice("vtollobbyid", 2)
            .AddChoice("jetbornelobbyid", 3)
            .AddChoice("systemmessageid", 4)
            .AddChoice("statusmessageid", 5)
            .AddChoice("saveconfig", 6)
            .AddChoice("botroleid", 7)
            .AddChoice("autoquery", 8)
            .WithType(ApplicationCommandOptionType.Integer)
        ).AddOption("value", ApplicationCommandOptionType.String, "The value to set the config item to.");

        public override string Action(SocketSlashCommand command)
        {
            if (HasPerms(command.User))
            {
                bool successful;
                ulong value;
                bool boolValue;

                switch (Convert.ToInt32(command.Data.Options.First().Value))
                {
                    case 1:
                        successful = ulong.TryParse((string)command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.delay = (int)value;
                            Log.LogInfo("Resetting query method because our delay has changed...", "SetConfig Handler", true);
                            Program.lobbyHandler.ResetQueryTimer();
                            return $"Successfully set delay to {value} seconds!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input into an integer.";
                        }

                    case 2:
                        successful = ulong.TryParse((string)command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.vtolLobbyChannelId = value;
                            return $"Successfully set VTOL lobby channel ID to {value}!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to an integer.";
                        }

                    case 3:
                        successful = ulong.TryParse((string)command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.jetborneLobbyChannelId = value;
                            return $"Successfully set Jetborne Racing lobby channel ID to {value}!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to an integer.";
                        }

                    case 4:
                        successful = ulong.TryParse((string)command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.systemMessageChannelId = value;
                            return $"Successfully set system message channel ID to {value}!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to an integer.";
                        }

                    case 5:
                        successful = ulong.TryParse((string) command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.statusMessageChannelId = value;
                            return $"Successfully set status message channel ID to {value}!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to an integer.";
                        }

                    case 6:
                        successful = bool.TryParse((string)command.Data.Options.ElementAt(1).Value, out boolValue);
                        if (successful)
                        {
                            if (boolValue)
                            {
                                Program.config.shouldSave = true;
                                return "Will save config on shutting down!";
                            }
                            else
                            {
                                Program.config.shouldSave = false;
                                return "Will not save config on shutting down!";
                            }
                        }
                        else return "Sorry, I couldn't translate your input to a true/false boolean.";

                    case 7:
                        successful = ulong.TryParse((string)command.Data.Options.ElementAt(1).Value, out value);
                        if (successful)
                        {
                            Program.config.botRoleId = value;
                            return $"Successfully set bot role ID to {value}!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to an integer.";
                        }

                    case 8:
                        successful = bool.TryParse((string) command.Data.Options.ElementAt(1).Value, out boolValue);
                        if (successful)
                        {
                            Program.config.autoQuery = boolValue;
                            return $"Successfully {(boolValue ? "enabled" : "disabled")} autoquery!";
                        }
                        else
                        {
                            return "Sorry, I couldn't translate your input to a true/false boolean.";
                        } 

                    default: throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\"");
                }
            }
            else return "Sorry, you don't have the permissions to use this command!";
        }
    }
}