using Steamworks.Data;

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

        ///<summary />
        public JetborneLobby(Lobby lobby)
        {
            LobbyName = lobby.GetData("name");
            OwnerName = lobby.GetData("ownerName");
            MemberCount = lobby.MemberCount;
            RaceLaps = lobby.GetData("raceLaps");
            CurrentLap = lobby.GetData("currentLap");
        }
    }
}