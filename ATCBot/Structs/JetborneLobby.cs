using System;
using SteamKit2;

namespace ATCBot.Structs
{
    public struct JetborneLobby
    {
        public string LobbyName;
        public string OwnerName;
        public int MemberCount;
        public string RaceLaps;
        public string CurrentLap;
        public string Map;

        public JetborneLobby(SteamMatchmaking.Lobby lobby)
        {
            LobbyName = lobby.Metadata["name"];
            OwnerName = lobby.Metadata["ownerName"];
            RaceLaps = lobby.Metadata["raceLaps"];
            Map = lobby.Metadata["map"];
            if (lobby.Metadata.TryGetValue("currentLap", out string value))
            {
                CurrentLap = value;
            }
            else
            {
                CurrentLap = string.Empty;
            }
            MemberCount = lobby.NumMembers;
        }
    }
}