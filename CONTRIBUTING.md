# Contributing to ATCBot

If you are reading this, thank you for considering contributing to ATCBot. As a small application, the rules for contributing are very lax, but are still enforced. Please take care to follow them to the best of your ability.

If anything is missing from this, please contact a repository owner, who is/are currently:
- Shadow

## Code

ATCBot is written in C#, using Visual Studio. It is contained under the ATCBot project, under the ATCBot solution. Please do not create new solutions or projects.

All files are written under the `ATCBot` namespace. All files contained in subfolders are under the `ATCBot.X` namespace, where `X` is the name of the folder. Please follow this format at all times!

As a public application, XML documentation is provided upon building the project. What this means is that **all** public members of a class must be documented appropriately! The compiler will warn you if you have not done so for any member. Please write at the very least a summary of the member's purpose. If you do not feel it needs one, write `<summary />` for the XML documentation and explain in the pull request why you do not believe it needs it.

## Hierarchy

### /

The base folder contains all application-critical classes, such as the Program, Config, and LobbyHandler classes (among others).

### /Commands

This folder contains all slash commands that the bot uses, as well as the parent class and the classes for building and handling their execution (elevated to the top by an underscore _). All commands must be under this folder, and their names should be written in PascalCase (a capital at the start of every word, no spaces between words). Please ensure all commands inherit from `ATCBot.Commands.Command` to ensure they are built correctly.

ATCBot currently does not support guild commands, only global commands.

### /Structs

This contains structs that are large enough that they need to be contained in their own file. Self-explanatory.

## Versioning

ATCBot follows semantic versioning. That is, all versions are written as `MAJOR.MINOR.PATCH`. `MAJOR` should be incremented for updates overhauling the entire program, making it completely incompatible with previous releases; `MINOR` should be incremented for updates adding new features, or generally changing how the program works without making its behavior incompatible with previous versions; and `PATCH` should be incremented for updates fixing bugs or other issues. Incrementing an earlier number resets all later numbers to 0 (e.g., 1.2.3 -> 2.0.0).

Prerelease builds are marked with a "p" after the end of the version.

Because official releases of the bot are sensitive, please do not change the version on your own, or at least without approval from a repository owner.

## GitHub Hierarchy

Currently, the repository supports two static branches: `dev` and `releases`. The former is the repository's default branch where all feature PRs should be merged to.

`releases` contains commits for every release of the bot and should only be updated when a version is ready to be released. Please do not make pull requests to this branch.

All other branches are feature branches by a repository owner and can be ignored.

## Code style

ATCBot's code style is flexible, and a comprehensive, rigid list of all necessary styles is unnecessary. In general, try to follow the existing style of the code and ask if you have any questions.

## Creating an issue/pull request

When creating an issue or pull request, please explain the problem/suggestion (if an issue) or the changes you have made (if a pull request). PRs should have all changes documented, no matter how small! Failure to do so will result in frowny faces.
