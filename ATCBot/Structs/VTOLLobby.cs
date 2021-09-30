using SteamKit2;

using System.Collections.Generic;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single VTOL VR lobby.
    /// </summary>
    public struct VTOLLobby
    {
        private string lobbyName;
        private string ownerName;
        private string ownerId;
        private string scenarioName;
        private string scenarioId;
        private string maxPlayers;
        private string feature;
        private string envIdx;
        private string gameVersion;
        private string briefingRoom;
        private string passwordHash;
        private int playerCount;

        /// <summary>
        /// The name of the lobby.
        /// </summary>
        public string LobbyName { get => lobbyName; private set => lobbyName = value; }

        /// <summary>
        /// The name of the owner of the lobby.
        /// </summary>
        public string OwnerName { get => ownerName; private set => ownerName = value; }

        /// <summary>
        /// The Steam ID of the owner of the lobby.
        /// </summary>
        public string OwnerId { get => ownerId; private set => ownerId = value; }

        /// <summary>
        /// The name of the scenario of the lobby.
        /// </summary>
        public string ScenarioName { get => scenarioName; private set => scenarioName = value; }

        /// <summary>
        /// The ID of the scenario of the lobby.
        /// </summary>
        public string ScenarioId { get => scenarioId; private set => scenarioId = value; }

        /// <summary>
        /// The maximum number of players in the lobby.
        /// </summary>
        public int MaxPlayers { get => int.Parse(maxPlayers); private set => maxPlayers = value.ToString(); }

        /// <summary>
        /// Unused.
        /// </summary>
        public string Feature { get => feature; private set => feature = value; }

        /// <summary>
        /// Unused.
        /// </summary>
        public int EnvIdx { get => int.Parse(envIdx); private set => envIdx = value.ToString(); }

        /// <summary>
        /// The version of the game the lobby is running.
        /// </summary>
        public string GameVersion { get => gameVersion; private set => gameVersion = value; }

        /// <summary>
        /// The type of briefing room being used.
        /// </summary>
        // TODO: enumerate types
        public int BriefingRoom { get => int.Parse(briefingRoom); private set => briefingRoom = value.ToString(); }

        /// <summary>
        /// The password hash of the lobby. 0 means it is public.
        /// </summary>
        public int PasswordHash { get => int.Parse(passwordHash); private set => passwordHash = value.ToString(); }

        /// <summary>
        /// The current amount of players in the lobby.
        /// </summary>
        public int PlayerCount { get => playerCount; private set => playerCount = value; }

        /// <summary>Create a lobby from a SteamKit2 lobby.</summary>
        public VTOLLobby(SteamMatchmaking.Lobby lobby)
        {
            if(lobby.Metadata.ContainsKey("name"))
            {
                Program.LogVerbose("Skipping modded lobby...");
                this = default;
                return;
            }
            bool successful = true;
            playerCount = lobby.NumMembers;
            if (!lobby.Metadata.TryGetValue("lName", out lobbyName)) successful = false;
            if (!lobby.Metadata.TryGetValue("oName", out ownerName)) successful = false;
            if (!lobby.Metadata.TryGetValue("oId", out ownerId)) successful = false;
            if (!lobby.Metadata.TryGetValue("scn", out scenarioName)) successful = false;
            if (!lobby.Metadata.TryGetValue("scID", out scenarioId)) successful = false;
            if (!lobby.Metadata.TryGetValue("maxP", out maxPlayers)) successful = false;
            if (!lobby.Metadata.TryGetValue("feature", out feature)) successful = false;
            if (!lobby.Metadata.TryGetValue("envIdx", out envIdx)) successful = false;
            if (!lobby.Metadata.TryGetValue("ver", out gameVersion)) successful = false;
            if (!lobby.Metadata.TryGetValue("brtype", out briefingRoom)) successful = false;
            if (!lobby.Metadata.TryGetValue("pwh", out passwordHash)) successful = false;
            if (!successful) Program.LogWarning("One or more keys could not be set correctly!", "VTOL VR Lobby Constructor", true);
        }
    }
}