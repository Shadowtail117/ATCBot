using ATCBot.Structs;

using System.Text;
using System.Threading.Tasks;

using Steamworks;
using System.Collections.Generic;
using System;
using Steamworks.Data;

namespace ATCBot
{
    class LobbyHandler
    {
        public TimeSpan delay = TimeSpan.FromSeconds(Program.config.delay);

        public List<VTOLLobby> vtolLobbies = new();
        public List<JetborneLobby> jetborneLobbies = new();

        public Program program;

        public async Task QueryTimer()
        {
            if(Program.shouldUpdate)
            {
                await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "Lobby Handler", "Updating..."));
                await GetData();
                await program.UpdateLobbyInformation();
            }
            else await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "Lobby Handler", "Skipping Update..."));
            await Task.Delay(delay);
            await QueryTimer();
        }

        private async Task GetData()
        {
            await Program.Log($"Getting lobbies at {DateTime.Now}.");

            SteamClient.Init(Program.vtolID);
            vtolLobbies.Clear();
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.RequestAsync();

            // If Lobbies are null that means there are 0 lobbies.
            if (lobbies != null)
            {
                foreach (Lobby lobby in lobbies)
                {
                    vtolLobbies.Add(new VTOLLobby(lobby));
                }
            }
            await ShutdownSteam();


            SteamClient.Init(Program.jetborneID);
            jetborneLobbies.Clear();
            lobbies = await SteamMatchmaking.LobbyList.RequestAsync();

            if (lobbies != null)
            {
                foreach (Lobby lobby in lobbies)
                {
                    jetborneLobbies.Add(new JetborneLobby(lobby));
                }
            }
            await ShutdownSteam();

            
        }

        private async Task ShutdownSteam()
        {
            // These delays are needed because an error happens if 
            // init and shutdown are ran at the same time
            await Task.Delay(TimeSpan.FromSeconds(1));
            SteamClient.Shutdown();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public LobbyHandler(Program program)
        {
            this.program = program;
        }
    }
}
