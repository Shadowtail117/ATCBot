using Steamworks.Data;

namespace ATCBot.Structs
{
    public struct JetborneLobby
    {
        public string LobbyName;
        public string OwnerName;
        public int MemberCount;
        public string RaceLaps;
        public string CurrentLap;
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