using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot.Commands
{
    class Help : Command
    {
        public override string Name { get; set; } = "help";
        public override SlashCommandBuilder Builder { get; set; } = new SlashCommandBuilder()
            .WithName("help")
            .WithDescription("Get assistance with a variety of topics.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("topic")
                .WithDescription("What do you need help with?")
                .WithRequired(true)
                .AddChoice("log", 1)
                .AddChoice("audio", 2)
                .AddChoice("oculus", 3)
                .AddChoice("verify", 4)
                .AddChoice("disablemodloader", 5)
                .AddChoice("sptomp", 6)
                .AddChoice("twoseat", 7)
                .WithType(ApplicationCommandOptionType.Integer)
            );

        public override bool Ephemeral { get; set; } = false;

        public override string Action(SocketSlashCommand command)
        {
            return Convert.ToInt32(command.Data.Options.First().Value) switch
            {
                1 => 
                "To get your player log, follow these steps:" +
                "\n1. Press and hold the Windows key and the R key. A menu should pop up in the bottom left of your screen." +
                "\n2. In this menu, paste the following: " + @"`%appdata%\..\LocalLow\Boundless Dynamics, LLC\VTOLVR`" +
                "\n3. A directory should appear. Inside should be a file named \"Player.log\", or just \"Player\" if you have file extensions hidden." +
                "\n4. Upload this file to Discord in the channel you are asked to do so.",

                2 => 
                "Some popular music/video downloaders can give you a file where the metadata and content of the music don't match. Essentially, " +
                "if the file says that it is stereo (it plays a different track in each ear) but it is actually mono (one track for both ears), " +
                "or vice versa, then VTOL VR will refuse to play the file." +
                "\nTo fix this, you can download an audio editor like Audacity and import the corrupted file. Then, re-export it as an mp3. This " +
                "will fix the content-header mismatch and VTOL VR should play the sound track like normal.",

                3 =>
                "For some people using Oculus-based headsets like the Quest, running VTOL VR in OculusVR instead of SteamVR can be more stable " +
                "or yield better performance. In order to do so:" +
                "\n1. Right click VTOL VR in your Steam library." +
                "\n2. Click on Properties in the dropdown menu." +
                "\n3. In the menu that pops up, enter \"oculus\" (without the quotes) in the text box at the bottom." +
                "\n4. Close the menu and launch the game.",

                4 =>
                "To verify your game files to fix corrupted files, follow these steps:" +
                "\n1. Right click VTOL VR in your Steam library." +
                "\n2. Click on Properties in the dropdown menu." +
                "\n3. In the menu that pops up, click \"Local Files\" in the sidebar on the left." +
                "\n4. Click \"Verify integrity of game files...\" at the bottom of the menu.",

                5 =>
                "To disable the VTOL VR Mod Loader, follow these steps:" +
                "\n1. In the Mod Loader's settings, under the Diagnostics section, click on the button next to \"Disable Mod Loader\"." +
                "\n2. Go into Steam and verify your game files. Type `/help verify` for help with this step.",

                6 =>
                "To convert an existing singleplayer mission to multiplayer:" +
                "\n1. Create a new singleplayer campaign." +
                "\n2. Import your mission into that campaign via the campaign editor." +
                "\n3. Convert the campaign to multiplayer via the campaign editor.",
                
                7 =>
                "According to BahamutoD:" +
                "\n> The only thing that a 2 [seater] jet without other new features would add is the ability to train someone." +
                "\n> If it was just a 2 seater 26b then no." +
                "\nPlease read <#756543573608104098> in regards to new vehicles.",  // Channel ID for #faq

                _ => throw new ArgumentException($"Invalid argument! \"{command.Data.Options.First().Value}\"")
            };
        }
    }
}
