using SteamKit2;

using System;
using System.Collections.Generic;

namespace ATCBot.Structs
{
    /// <summary>
    /// Represents a single Jetborne Racing lobby.
    /// </summary>
    public struct JetborneLobby : IComparable<JetborneLobby>
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
        private int maxPlayers;

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

        /// <summary>
        /// The max amount of players in the lobby.
        /// </summary>
        public int MaxPlayers { get => maxPlayers; private set => maxPlayers = value; }


        /// <summary>Create a lobby from a SteamKit2 lobby.</summary>>
        public JetborneLobby(SteamMatchmaking.Lobby lobby)
        {
            if (!lobby.Metadata.ContainsKey("currentLap") || !lobby.Metadata.ContainsKey("ownername"))
            {
                Log.LogVerbose("Skipping incomplete lobby...", "VTOL VR Lobby Constructor");
                this = default;
                return;
            }

            List<string> badKeys = new();

            playerCount = lobby.NumMembers;
            maxPlayers = lobby.MaxMembers;

            if (!lobby.Metadata.TryGetValue("name", out lobbyName))
                badKeys.Add("name");

            if (!lobby.Metadata.TryGetValue("ownername", out ownerName))
                badKeys.Add("ownername");

            if (!lobby.Metadata.TryGetValue("currentLap", out currentLap))
                badKeys.Add("currentLap");

            if (!lobby.Metadata.TryGetValue("raceLaps", out raceLaps))
                badKeys.Add("raceLaps");

            if (!lobby.Metadata.TryGetValue("map", out map))
                badKeys.Add("map");

            if (!lobby.Metadata.TryGetValue("leaderboardsEnabled", out leaderboardsEnabled))
                badKeys.Add("leaderboardsEnabled");

            if (!lobby.Metadata.TryGetValue("blackoutMode", out blackoutMode))
                badKeys.Add("blackoutMode");

            if (!lobby.Metadata.TryGetValue("gameVersion", out gameVersion))
                badKeys.Add("gameVersion");

            if (!lobby.Metadata.TryGetValue("allowItems", out allowItems))
                badKeys.Add("allowItems");

            if (!lobby.Metadata.TryGetValue("playerCollisions", out playerCollisions))
                badKeys.Add("playerCollisions");

            if (!lobby.Metadata.TryGetValue("wallMode", out wallMode))
                badKeys.Add("wallMode");


            //if (Blacklist.blacklist.Contains(long.Parse(ownerId)))
            //{
            //    Log.LogVerbose("Skipping blacklisted lobby...");
            //    this = default;
            //    return;
            //}

            if (badKeys.Count > 0)
            {
                Log.LogWarning($"One or more keys could not be set correctly! \"{string.Join(", ", badKeys.ToArray())}\"", "JBR Lobby Constructor", true);
                this = default;
            }
            Log.LogDebug($"Found JBR Lobby | Name: {LobbyName} , Owner: {OwnerName} , Map: {Map} , Players: {PlayerCount}", "JBR Lobby Constructor");
        }

        /// <inheritdoc/>
        public int CompareTo(JetborneLobby other)
        {
            //if our lobby is full and the other isn't, we go after them
            if (playerCount == MaxPlayers && other.playerCount != other.MaxPlayers)
            {
                return 1;
            }
            //if we have the same players, sort alphabetically
            else if (playerCount == other.playerCount)
            {
                return lobbyName.CompareTo(other.lobbyName);
            }
            //if we have more players, we go after them
            else if (playerCount > other.playerCount)
            {
                return 1;
            }
            //if we have less players, we go before them
            else if (playerCount < other.playerCount)
            {
                return -1;
            }
            //if we have made it here then something has gone wrong with the comparison process
            throw new Exception("Could not compare lobbies!");
        }
    }
}