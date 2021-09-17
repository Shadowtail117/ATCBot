using SteamKit2;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single VTOL VR lobby.
    /// </summary>
    public struct VTOLLobby
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
        /// The name of the scenario the lobby is currently running.
        /// </summary>
        public string ScenarioText;

        /// <summary>
        /// How many people are in the lobby.
        /// </summary>
        public int MemberCount;

        public VTOLLobby(SteamMatchmaking.Lobby lobby)
        {
            LobbyName = lobby.Metadata["lName"];
            OwnerName = lobby.Metadata["oName"];
            ScenarioText = lobby.Metadata["scn"];
            MemberCount = lobby.NumMembers;
        }
    }
}