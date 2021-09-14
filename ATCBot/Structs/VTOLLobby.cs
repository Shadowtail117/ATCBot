using SteamKit2;

namespace ATCBot.Structs
{
    public struct VTOLLobby
    {
        public string LobbyName;
        public string OwnerName;
        public string ScenarioText;
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