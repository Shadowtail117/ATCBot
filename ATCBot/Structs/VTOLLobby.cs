using Steamworks.Data;

namespace ATCBot.Structs
{
    public struct VTOLLobby
    {
        public string LobbyName;
        public string OwnerName;
        public string ScenarioText;
        public int MemberCount;

        public VTOLLobby(Lobby lobby)
        {
            LobbyName = lobby.GetData("lName");
            OwnerName = lobby.GetData("oName");
            ScenarioText = lobby.GetData("scn");
            MemberCount = lobby.MemberCount;
        }
    }
}