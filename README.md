# ATCBot v1.0.0
[![.NET](https://github.com/Shadowtail117/ATCBot/actions/workflows/release.yml/badge.svg)](https://github.com/Shadowtail117/ATCBot/actions/workflows/release.yml)

ATCBot is a Discord bot made for the VTOL VR / Jetborne Racing community to fetch and display lobby information to help players identify good times to get on.

## Design

ATCBot will publish lobby information using a singular message, preferably within its own channel(s) on the server. It will fetch lobby information continuously except when told to stop, and will wait a variable amount of time per second based on the configuration that it is running.

Most variables of the bot are changeable using slash commands. See below.

### Steamworks

ATCBot uses Steamworks to fetch lobby information. Steamworks requires a logged in Steam account to be present on the machine hosting it. If there isn't, ATCBot will not work properly!

## Commands

ATCBot currently features the following commands:

| Command         | Parameters                                                   | Description                                    | Permissions Required |
| -------         | ----------                                                   | -----------                                    | -------------------- |
| `version`       | None                                                         | Gets the version of the bot, as updated by me. | None                 |
| `startupdating` | None                                                         | Starts updating the lobby information.         | Manage Server        |
| `stopupdating`  | None                                                         | Stops updating the lobby information           | Manage Server        |
| `getconfig`     | `config`: The config item to get.                            | Gets the value of a config item.               | Manage Server        |
| `setconfig`     | `config`: The config item to set. `value`: The value to set. | Sets the value of a config item.               | Manage Server        |
| `shutdown`      | None                                                         | Shuts down the bot. Requires a manual restart. | Manage Server        |

#### getconfig
Valid arguments for `getconfig`:
- `delay` - The delay **in seconds** between updates to the lobby messages, on top of network processing times.
- `updating` - Whether or not the bot is currently updating the lobby messages.
- `vtolchannelid` - The channel ID set for the bot to post VTOL VR lobby information.
- `jetbornechannelid` - The channel ID set for the bot to post Jetborne Racing lobby information.

### setconfig
Valid arguments for `setconfig`'s first parameter:
- `delay` - The delay **in seconds** between updates to the lobby messages, on top of network processing times.
- `vtolchannelid` - The channel ID set for the bot to post VTOL VR lobby information.
- `jetbornechannelid` - The channel ID set for the bot to post Jetborne Racing lobby information.
- `resetcommands` - Whether or not to rebuild all slash commands on the next start. Only use this if something has gone horribly wrong!
- `saveconfig` - Whether or not the configuration will be saved upon exiting. Defaults to true every time.

The second argument for `setconfig` is any string. It will try to parse it into an acceptable argument for the corresponding first argument -- a `ulong` (integer) for `delay`, `vtolchannelid`, and `jetbornechannelid`, and a boolean for `resetcommands` and `saveconfig`.

## Hosting

ATCBot intrinsically requires a configuration in order to work to the host's and server's needs. Upon running the bot for the first time, a `Config` folder will be created in the directory the .exe was run in, and the program will self-terminate.

In the folder is `config.cfg` and `token.txt`. `config.cfg` represents a JSON version of the bot's current configuration and should not be directly edited unless you know what you are doing. A corrupted config will make the bot do bad things! (Mostly just crash.)

`token.txt` is where you should put the bot's token, which you should have gotten from the Discord developer portal. This is done not only so that I don't doxx my own bot but also so other people can use their own bots to host ATCBot's code. You must set `token.txt` before running the bot otherwise it will either close after realizing it has been duped, or crash because of a malformed token.

Unless you are confident enough to edit `config.cfg` yourself, you should change all public configuration variables using the in-built slash commands after running the bot. The configuration will automatically save if you have not specified not to, and you exit using the `shutdown` command. Closing it via the console window will not save the config.

### Monitoring

As a console-line application, ATCBot will open a console window whenever it is running. You cannot input into this window, and it is meant for monitoring/diagnostic purposes. Closing it will close the bot. If you don't want to see it, minimize it.

## Credits

ATCBot is developed by myself (Shadow) with very kind contributions from Marsh.
