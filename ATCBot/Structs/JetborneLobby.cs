using SteamKit2;
using System.Collections.Generic;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single Jetborne Racing lobby.
    /// </summary>
    public struct JetborneLobby
    {
        /// <summary>
        /// An empty lobby.
        /// </summary>
        public static readonly JetborneLobby Empty = new() { LobbyName = "", MemberCount = 0, OwnerName = "", RaceLaps = "", CurrentLap = "", Map = "" };

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
        /// The current lap.
        /// </summary>
        public string CurrentLap;

        /// <summary>
        /// The name of the map being used.
        /// </summary>
        public string Map;

        /// <summary />
        public JetborneLobby(SteamMatchmaking.Lobby lobby)
        {
            try
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
            catch(KeyNotFoundException e)
            {
                Program.LogError("Could not parse lobby metadata!", e, "JBR Lobby Constructor");
                this = Empty;
            }
        }
    }
}