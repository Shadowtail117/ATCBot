using System;
using SteamKit2;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single Jetborne Racing lobby.
    /// </summary>
    public struct JetborneLobby
    {
        /// <summary>
        /// The name of the lobby.
        /// </summary>
        public string LobbyName;

        /// <summary>
        /// The name of the owner of the lobby.
        /// </summary>
        public string OwnerName;

        /// <summary>
        /// How many people are in the lobby.
        /// </summary>
        public int MemberCount;

        /// <summary>
        /// How many laps are in the race.
        /// </summary>
        public string RaceLaps;

        /// <summary>
        /// The current lap/
        /// </summary>
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