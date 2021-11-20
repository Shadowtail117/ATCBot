using SteamKit2;

using System;
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
        private string ld_GameState;
        private string mUtc;
        private int playerCount;

        internal bool valid;

        /// <summary>
        /// Represents the version status of a lobby.
        /// </summary>
        public enum FeatureType
        {
            /// <summary>Main branch</summary>
            f,
            /// <summary>Public testing branch</summary>
            p,
            /// <summary>Modded version</summary>
            m
        }

        /// <summary>
        /// Represents the time of day of a lobby.
        /// </summary>
        public enum EnvType
        {
            /// <summary />
            Day,
            /// <summary />
            Morning,
            /// <summary />
            Night
        }

        /// <summary>
        /// The current state of a lobby.
        /// </summary>
        public enum GameState
        {
            /// <summary>In briefing (pre-game).</summary>
            Briefing,
            /// <summary>In the mission (in-game).</summary>
            Mission,
            /// <summary>In debriefing (post-game).</summary>
            Debrief
        }

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
        /// The type of lobby - main branch, PTB, or modded.
        /// </summary>
        public FeatureType Feature { get => Enum.Parse<FeatureType>(feature); private set => feature = value.ToString(); }

        /// <summary>
        /// Whether it is day, morning, or night.
        /// </summary>
        public EnvType EnvIdx { get => (EnvType) int.Parse(envIdx); private set => envIdx = value.ToString(); }

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

        /// <summary>
        /// Whether the game has started or not.
        /// </summary>
        public GameState LobbyGameState { get => Enum.Parse<GameState>(ld_GameState); private set => ld_GameState = value.ToString(); }

        /// <summary>
        /// The mission elapsed time.
        /// </summary>
        public string MET { get 
            {
                ElapsedMinutes(out int METHours, out int METMinutes);
                if(METHours == -1 || METMinutes == -1)
                {
                    Log.LogDebug("Could not convert lobby MET, likely in (de)briefing.", LobbyName);
                }
                return $"{METHours}:{METMinutes:00}";
            }
            private set => mUtc = value;
        }

        private void ElapsedMinutes(out int hours, out int minutes)
        {
            if (!string.IsNullOrEmpty(mUtc))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                if (DateTime.TryParse(mUtc, culture, System.Globalization.DateTimeStyles.None, out var startTime))
                {
                    var endTime = DateTime.UtcNow;
                    var delta = endTime - startTime;
                    minutes = delta.Minutes;
                    hours = delta.Hours;
                    return;
                }
            }

            minutes = -1;
            hours = -1;
        }

        internal bool METValid() => MET != "-1:-01";

        /// <summary>
        /// Whether or not this lobby is password protected.
        /// </summary>
        public bool PasswordProtected() => PasswordHash != 0;

        /// <summary>Create a lobby from a SteamKit2 lobby.</summary>
        public VTOLLobby(SteamMatchmaking.Lobby lobby)
        {
            if (lobby.Metadata.ContainsKey("name"))
            {
                Log.LogVerbose("Skipping modded lobby...", "VTOL VR Lobby Constructor");
                this = default;
                return;
            }
            if (!lobby.Metadata.ContainsKey("scn"))
            {
                Log.LogVerbose("Skipping incomplete lobby...", "VTOL VR Lobby Constructor");
                this = default;
                return;
            }

            List<string> badKeys = new();

            playerCount = lobby.NumMembers;

            if (!lobby.Metadata.TryGetValue("lName", out lobbyName))
                badKeys.Add("lName");

            if (!lobby.Metadata.TryGetValue("oName", out ownerName))
                badKeys.Add("oName");

            if (!lobby.Metadata.TryGetValue("oId", out ownerId))
                badKeys.Add("oId");

            if (!lobby.Metadata.TryGetValue("scn", out scenarioName))
                badKeys.Add("scn");

            if (!lobby.Metadata.TryGetValue("scID", out scenarioId))
                badKeys.Add("scID");

            if (!lobby.Metadata.TryGetValue("maxP", out maxPlayers))
                badKeys.Add("maxP");

            if (!lobby.Metadata.TryGetValue("feature", out feature))
                badKeys.Add("feature");

            if (!lobby.Metadata.TryGetValue("envIdx", out envIdx))
                badKeys.Add("envIdx");

            if (!lobby.Metadata.TryGetValue("ver", out gameVersion))
                badKeys.Add("ver");

            if (!lobby.Metadata.TryGetValue("brtype", out briefingRoom))
                badKeys.Add("brtype");

            if (!lobby.Metadata.TryGetValue("pwh", out passwordHash))
                badKeys.Add("pwh");

            if (!lobby.Metadata.TryGetValue("gState", out ld_GameState))
                badKeys.Add("gState");

            if (!lobby.Metadata.TryGetValue("mUtc", out mUtc))
                Log.LogVerbose("Could not find value 'mUtc', this lobby probably hasn't started yet.");

            if (badKeys.Count > 0)
            {
                Log.LogWarning($"One or more keys could not be set correctly! \"{string.Join(", ", badKeys.ToArray())}\"", "VTOL VR Lobby Constructor", true);
                valid = false;
            }
            else
                valid = true;
            Log.LogDebug($"Found VTOL Lobby | Name: {LobbyName} , Owner: {OwnerName} , Scenario: {ScenarioName} , Players: {PlayerCount}/{MaxPlayers} , PP: {PasswordProtected()}",
                "VTOL VR Lobby Constructor");
        }
    }
}