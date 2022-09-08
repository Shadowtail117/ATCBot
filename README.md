# ATCBot

[![Build](https://github.com/Shadowtail117/ATCBot/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Shadowtail117/ATCBot/actions/workflows/dotnet.yml) [![CodeQL](https://github.com/Shadowtail117/ATCBot/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Shadowtail117/ATCBot/actions/workflows/codeql-analysis.yml) ![Release](https://img.shields.io/github/v/release/Shadowtail117/ATCBot?label=Release) ![Pre-release](https://img.shields.io/github/v/release/Shadowtail117/ATCBot?include_prereleases&label=Pre-release)

ATCBot is a Discord bot made for the VTOL VR / Jetborne Racing community to fetch and display lobby information to help players identify good times to get on.

## Design

ATCBot will publish lobby information using a singular message per game, preferably within its own channel(s) on the server, as well as a message depicting its current status. It will fetch lobby information continuously except when told to stop, and will wait a variable amount of time between updates based on the configuration that it is running.

Most variables of the bot are changeable using slash commands. See below.

## Commands

ATCBot currently features the following commands:

| Command                  | Parameters                                                                                                                                       | Description                                           | Ephemeral | Permissions Required |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------------- | --------- | -------------------- |
| `version`                | None                                                                                                                                             | Gets the local version of the bot, as updated by me.  | Yes       | None                 |
| `startupdating`          | None                                                                                                                                             | Starts updating the lobby information.                | Yes       | Bot Role             |
| `stopupdating`           | None                                                                                                                                             | Stops updating the lobby information.                 | Yes       | Bot Role             |
| `getconfig`              | `config`: The config item to get.                                                                                                                | Gets the value of a config item.                      | Yes       | Bot Role             |
| `setconfig`              | `config`: The config item to set.<br/>`value`: The value to set.                                                                                 | Sets the value of a config item.                      | Yes       | Bot Role             |
| `shutdown`               | None                                                                                                                                             | Shuts down the bot. Requires a manual restart.        | No        | Bot Role             |
| `refresh`                | None                                                                                                                                             | Forces the bot to update lobby information messages.  | No        | Bot Role             |
| `setstatusmessage`       | `status`: The type of status, either online, offline, or custom.<br/>`custommessage`: If `status` was set to custom, the custom message to show. | Changes the status message of the bot, if set.        | Yes       | Bot Role             |
| `rebuildcommands`        | None                                                                                                                                             | Forces the bot to rebuild slash commands.             | Yes       | Bot Owner            |
| `setlogverbosity`        | `verbosity`: The verbosity to set.                                                                                                               | Sets the verbosity of logs.                           | Yes       | Bot Owner            |
| `lookup`                 | `name`: The exact name of the lobby host to lookup.                                                                                              | Looks up the SteamID of the host of a lobby.          | Yes       | Bot Role             |
| `blacklist`              | `id`: The SteamID to blacklist.                                                                                                                  | Blacklists a SteamID from appearing in the list.      | Yes       | Bot Role             |
| `help`                   | `topic`: One of several topics to receive help about.                                                                                            | Gives you help about a topic.                         | No        | None                 |

### Bot Role
"Bot role" refers to a role you can set in the server that the bot will check if a user is in for restricted commands. If the role is not set (note: being set incorrectly does not count!), then the bot will instead check if the user has Manage Server/Administrator permissions.

If the bot role is set, then the bot will check if the user either has that role, or has Administrator permissions.

`rebuildcommands` is a special command that has side effects and as such is only available to the person who is marked as the bot owner in the configuration.

#### getconfig
Valid arguments for `getconfig`:
- `delay` - The delay **in seconds** between updates to the lobby messages, on top of network processing times.
- `updating` - Whether or not the bot is currently updating the lobby messages.
- `vtolchannelid` - The channel ID set for the bot to post VTOL VR lobby information.
- `jetbornechannelid` - The channel ID set for the bot to post Jetborne Racing lobby information.
- `systemmessageid` - The channel ID set for the bot to post system messages.
- `botroleid` - The role ID set for the bot to check permission to use restricted commands.
- `autoquery` - Whether or not the bot will immediately begin lobby queries when it is ready.

#### setconfig
Valid arguments for `setconfig`'s first parameter:
- `delay` - The delay **in seconds** between updates to the lobby messages, on top of network processing times.
- `vtolchannelid` - The channel ID set for the bot to post VTOL VR lobby information.
- `jetbornechannelid` - The channel ID set for the bot to post Jetborne Racing lobby information.
- `systemmessageid` - The channel ID set for the bot to post system messages.
- `botroleid` - The role ID set for the bot to check permission to use restricted commands.
- `autoquery` - Whether or not the bot will immediately begin lobby queries when it is ready.

The second argument for `setconfig` is any text. It will try to parse it into an acceptable argument for the corresponding first argument -- a `ulong` (integer) for `delay`, `vtolchannelid`, `jetbornechannelid`, and `systemmessageid`, and a boolean for `saveconfig`.

#### setlogverbosity
Valid arguments for `setlogverbosity`:
- `normal` - Displays only informational, warning, error, and critical messages. Does not display verbose or debug messages.
- `verbose` - Displays informational, warning, error, critical, and verbose messages. Does not display debug messages.
- `debug` - Displays all messages.

#### setstatusmessage

Sets the content of the bot's status message. If set to `Online` or `Offline`, the message's content will be just that. Additionally, it will automatically try to change to `Offline` when shutting down and `Online` when starting up.

If `Custom` is selected, a custom status message will be displayed instead and will remain until changed, even after shutting down and restarting the bot.

#### blacklist

Using the `lookup` and `blacklist` commands, you can filter out obscene lobby or host names. 

Using the `lookup` command, you can obtain the SteamID of the host of an unwanted lobby. Paste that ID into the `blacklist` command, and their ID will be added to a blacklist effective immediately. If a person whose SteamID is blacklisted hosts a lobby, their lobby will never show up in the multiplayer lobbies list.

To remove someone from the whitelist, identify their SteamID, go to `blacklist.txt` in the Config folder, and remove it. It will take effect on the next restart.

## Hosting

ATCBot intrinsically requires a configuration in order to work to the host's and server's needs. Upon running the bot for the first time, a `Config` folder will be created in the directory the .exe was run in, and the program will self-terminate.

In the folder is `config.cfg` and `token.txt`. `config.cfg` represents a JSON version of the bot's current configuration and should not be directly edited unless you know what you are doing. A corrupted config will make the bot do bad things! (Mostly just crash.) **The only exception to this is editing the field marked `botOwnerId`, whose value should be replaced with your user ID.**

`token.txt` is where you should put the bot's token, which you should have gotten from the Discord developer portal. This is done not only so that I don't doxx my own bot but also so other people can use their own bots to host ATCBot's code. You must set `token.txt` before running the bot otherwise it will either close after realizing it has been duped, or crash because of a malformed token.

Unless you are confident enough to edit `config.cfg` yourself, you should change all public configuration variables using the in-built slash commands after running the bot. Changes made using commands will be saved automatically.

### SteamKit

ATCBot uses SteamKit to fetch lobby information. Upon setting the bot's token above, running it a second time will generate a `steam.json` in the same `Config` folder. In this, input a Steam account's username and password (2FA is not recommended). The bot will use these credentials to log into the account to fetch the lobby information.

If you have Steam Guard Email set up, running the bot (assuming correct credentials) will send an email to the address on file for the account. Input the code to the `AuthCode` field. You should only have to do this once.

If you have Steam Guard Mobile set up, you will need to input the **current** code to `TwoFactorAuthCode` *every time you start the program.*

While it may seem obvious, the account must own VTOL VR and Jetborne Racing to work correctly!

**Note: This configuration file is stored locally on the system and is not uploaded anywhere or used anywhere other than for the purposes of fetching lobby information. I cannot see your username or password and don't want to anyway.**

### Monitoring

As a console-line application, ATCBot will open a console window whenever it is running. You cannot input into this window, and it is meant for monitoring/diagnostic purposes. Closing it will close the bot. If you don't want to see it, minimize it.

ATCBot will optionally input into a system messages channel any logs that are considered important. These logs are the same as the ones in the console window and you do not lose information by not setting the channel ID. However, the bot will warn you that it is not set.

#### Watchdog

ATCBot has a built-in watchdog. This means that it will monitor its own queries to Steamworks for lobby information and will detect if for some reason it has stopped receiving information from it. If it detects this to happen 5 times in a row, it will conclude that something unrecoverable has happened and will attempt a hard reset. If this does not work, it will terminate the application entirely along with attempting to notify the bot role if set. Otherwise, it will continue as normal.

## Credits

ATCBot is developed by myself (Shadow) with very kind contributions from Marsh.
