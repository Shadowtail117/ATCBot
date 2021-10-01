using SteamKit2;

using System;
using System.Collections.Generic;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single Jetborne Racing lobby.
    /// </summary>
    public struct JetborneLobby
    {
        private string lobbyName;
        private string ownerName;
        private string currentLap;
        private string raceLaps;
        private string map;
        private string leaderboardsEnabled;
        private string blackoutMode;
        private string gameVersion;
        private string allowItems;
        private string playerCollisions;
        private string wallMode;
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
        /// The current lap, 0 if still in the lobby.
        /// </summary>
        public int CurrentLap { get => int.Parse(currentLap); private set => currentLap = value.ToString(); }

        /// <summary>
        /// The total amount of laps.
        /// </summary>
        public int RaceLaps { get => int.Parse(raceLaps); private set => raceLaps = value.ToString(); }

        /// <summary>
        /// The name of the map being used.
        /// </summary>
        public string Map { get => map; private set => map = value; }

        /// <summary>
        /// Whether or not leaderboards are enabled for this lobby.
        /// </summary>
        public bool LeaderboardsEnabled { get => bool.Parse(leaderboardsEnabled); private set => leaderboardsEnabled = value.ToString(); }

        /// <summary>
        /// The type of blackout mode being used.
        /// </summary>
        public string BlackoutMode { get => blackoutMode; private set => blackoutMode = value; }

        /// <summary>
        /// The version of the game the lobby is running.
        /// </summary>
        public string GameVersion { get => gameVersion; private set => gameVersion = value; }

        /// <summary>
        /// Whether or not items are allowed.
        /// </summary>
        public bool AllowItems { get => bool.Parse(allowItems); private set => allowItems = value.ToString(); }

        /// <summary>
        /// Whether or not player collisions are allowed.
        /// </summary>
        public bool PlayerCollisions { get => bool.Parse(playerCollisions); private set => playerCollisions = value.ToString(); }

        /// <summary>
        /// The type of wall mode being used.
        /// </summary>
        public string WallMode { get => wallMode; private set => wallMode = value; }

        /// <summary>
        /// The current amount of players in the lobby.
        /// </summary>
        public int PlayerCount { get => playerCount; private set => playerCount = value; }

        /// <summary>Create a lobby from a SteamKit2 lobby.</summary>>
        public JetborneLobby(SteamMatchmaking.Lobby lobby)
        {
            List<string> badKeys = new();
            bool successful = true;
            playerCount = lobby.NumMembers;
            if (!lobby.Metadata.TryGetValue("name", out lobbyName))
            {
                successful = false;

            }

            if (!lobby.Metadata.TryGetValue("ownername", out ownerName))
            {
                successful = false;
                badKeys.Add("ownername");
            }

            if (!lobby.Metadata.TryGetValue("currentLap", out currentLap))
            {
                successful = false;
                badKeys.Add("currentLap");
            }

            if (!lobby.Metadata.TryGetValue("raceLaps", out raceLaps))
            {
                successful = false;
                badKeys.Add("raceLaps");
            }

            if (!lobby.Metadata.TryGetValue("map", out map))
            {
                successful = false;
                badKeys.Add("map");
            }

            if (!lobby.Metadata.TryGetValue("leaderboardsEnabled", out leaderboardsEnabled))
            {
                successful = false;
                badKeys.Add("leaderboardsEnabled");
            }

            if (!lobby.Metadata.TryGetValue("blackoutMode", out blackoutMode))
            {
                successful = false;
                badKeys.Add("blackoutMode");
            }

            if (!lobby.Metadata.TryGetValue("gameVersion", out gameVersion))
            {
                successful = false;
                badKeys.Add("gameVersion");
            }

            if (!lobby.Metadata.TryGetValue("allowItems", out allowItems))
            {
                successful = false;
                badKeys.Add("allowItems");
            }

            if (!lobby.Metadata.TryGetValue("playerCollisions", out playerCollisions))
            {
                successful = false;
                badKeys.Add("playerCollisions");
            }

            if (!lobby.Metadata.TryGetValue("wallMode", out wallMode))
            {
                successful = false;
                badKeys.Add("wallMode");
            }

            if (!successful) Log.LogWarning($"One or more keys could not be set correctly! \"{string.Join(", ", badKeys.ToArray())}\"", "JBR Lobby Constructor", true);
        }
    }
}