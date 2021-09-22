using SteamKit2;

using System.Collections.Generic;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single VTOL VR lobby.
    /// </summary>
    public struct VTOLLobby
    {
        /// <summary>
        /// An empty lobby.
        /// </summary>
        public static readonly VTOLLobby Empty = new() { LobbyName = "", MemberCount = 0, OwnerName = "", ScenarioText = "", PasswordProtected = false };

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

        /// <summary>
        /// Whether or not the lobby is password protected.
        /// </summary>
        public bool PasswordProtected;

        /// <summary />
        public VTOLLobby(SteamMatchmaking.Lobby lobby)
        {
            try
            {
                _ = lobby.Metadata["name"];
                //If we get this far, this means this IS an MP mod lobby
                Program.LogVerbose("Skipping over modded lobby...", "VTOL Lobby Constructor");
                this = Empty;
                return;
            }
            catch (KeyNotFoundException) //If we catch an error, this means this is NOT an MP mod lobby
            {

            }

            try
            {
                LobbyName = lobby.Metadata["lName"];
                OwnerName = lobby.Metadata["oName"];
                ScenarioText = lobby.Metadata["scn"];
                MemberCount = lobby.NumMembers;
                PasswordProtected = !lobby.Metadata["pwh"].Equals("0"); //0 = public
            }
            catch (KeyNotFoundException e) //If we catch this, this means there is an actual issue
            {
                Program.LogError("Could not parse lobby metadata!", e, "VTOL Lobby Constructor");
                this = Empty;
            }
        }
    }
}