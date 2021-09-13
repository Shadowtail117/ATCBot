# ATCBot v0.1.0
[![.NET](https://github.com/Shadowtail117/ATCBot/actions/workflows/release.yml/badge.svg)](https://github.com/Shadowtail117/ATCBot/actions/workflows/release.yml)

ATCBot is a Discord bot made for the VTOL VR / Jetborne Racing community to fetch and display lobby information to help players identify good times to get on.

## Design

ATCBot will publish lobby information using a singular message, preferably within its own channel(s) on the server. It will fetch lobby information continuously except when told to stop, and will wait a variable amount of time per seconds based on the configuration that it is running.

Most variables of the bot are changeable using slash commands. See below.

## Commands

ATCBot currently features the following commands:

| Command         | Parameters | Description                                    | Permissions Required |
| -------         | ---------- | -----------                                    | -------------------- |
| `version`       | None       | Gets the version of the bot, as updated by me. | *None*               |
| `startupdating` | None       | Starts updating the lobby information.         | Manage Server        |
| `stopupdating`  | None       | Stops updating the lobby information           | Manage Server        |

#### Planned Commands

- Config get/set
- Change channel to update lobby info in for either game

## Hosting

ATCBot intrinsically requires a configuration in order to work to the host's and server's needs. Upon running the bot for the first time, a `Config` folder will be created in the directory the .exe was run in, and the program will self-terminate.

In the folder is `config.cfg` and `token.txt`. `config.cfg` represents a JSON version of the bot's current configuration and should not be directly edited unless you know what you are doing. A corrupted config will make the bot do bad things! (Mostly just crash.)

`token.txt` is where you should put the bot's token, which you should have gotten from the Discord developer portal. This is done not only so that I don't doxx my own bot but also so other people can use their own bots to host ATCBot's code. You must set `token.txt` before running the bot otherwise it will either close after realizing it has been duped, or crash because of a malformed token.

Unless you are confident enough to edit `config.cfg` yourself, you should change all public configuration variables using the in-built slash commands after running the bot.

### Monitoring

As a console-line application, ATCBot will open a console window whenever it is running. You cannot input into this window, and it is meant for monitoring/diagnostic purposes. Closing it will close the bot. If you don't want to see it, minimize it.

## Credits

ATCBot is developed by myself (Shadow) with very kind contributions from Marsh.
