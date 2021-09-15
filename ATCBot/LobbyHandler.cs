using ATCBot.Structs;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SteamKit2;

namespace ATCBot
{
    class LobbyHandler
    {
        public TimeSpan delay = TimeSpan.FromSeconds(Program.config.delay);
        public TimeSpan steamTimeout = TimeSpan.FromSeconds(Program.config.steamTimeout);

        public List<VTOLLobby> vtolLobbies = new();
        public List<JetborneLobby> jetborneLobbies = new();

        public Program program;
        
        private static bool running;
        private static bool loggedIn;
        
        private SteamClient client;
        private CallbackManager manager;
        private SteamUser user;
        private SteamMatchmaking matchmaking;

        public async Task QueryTimer()
        {
            if(Program.shouldUpdate)
            {
                await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "Lobby Handler", "Updating..."));
                await GetLobbies();
                await program.UpdateLobbyInformation();
            }
            else await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Verbose, "Lobby Handler", "Skipping Update..."));
            await Task.Delay(delay);
            await QueryTimer();
        }
        
        public LobbyHandler(Program program)
        {
            this.program = program;
            SetupSteam();
        }
        
        private async void SetupSteam()
        {
            client = new SteamClient();
            manager = new CallbackManager(client);
            matchmaking = client.GetHandler<SteamMatchmaking>();
            user = client.GetHandler<SteamUser>();

            SetupCallbacks();

            await Program.Log("Connecting to steam");
            running = true;
            client.Connect();

            // This needs to run on another thread to continue the program
            Task.Run(() =>
            {
                while (running)
                {
                    manager.RunWaitCallbacks(steamTimeout);
                }
            });

        }

        private void SetupCallbacks()
        {
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);
        }
        
        private async void OnConnected(SteamClient.ConnectedCallback callback)
        {
            await Program.Log($"Connected to steam. Logging into {SteamConfig.Config.SteamUserName}");

            byte[] sentryHash = null;
            string sentryPath = Path.Combine(Directory.GetCurrentDirectory(), "sentry.bin");
            if (File.Exists(sentryPath))
            {
                byte[] sentryFile = File.ReadAllBytes( "sentry.bin" );
                sentryHash = CryptoHelper.SHAHash( sentryFile );
            }
            
            user.LogOn(new SteamUser.LogOnDetails()
            {
                Username = SteamConfig.Config.SteamUserName,
                Password = SteamConfig.Config.SteamPassword,
                AuthCode = SteamConfig.Config.AuthCode,
                TwoFactorCode = SteamConfig.Config.TwoFactorAuthCode,
                SentryFileHash = sentryHash
            });
        }
        
        private async void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            await Program.Log("Disconnected from Steam");
            running = false;
        }
        
        private async void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            bool hasSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool hasF2A = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (hasSteamGuard)
            {
                await Program.Log($"Emailed Auth Code was invalid. Please update the steam.json and try again");
                return;
            }
            
            if (hasF2A)
            {
                await Program.Log("F2A code was invalid. Please update the steam.json and try again");
                return;
            }
            
            if (callback.Result != EResult.OK)
            {
                running = false;
                await Program.Log($"Failed to log into steam. {callback.Result} {callback.ExtendedResult}");
                return;
            }

            await Program.Log("Logged into steam account");
            loggedIn = true;
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            loggedIn = false;
        }
        
        private void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            Program.Log("Updating sentryfile...");

            // write out our sentry file
            // ideally we'd want to write to the filename specified in the callback
            // but then this sample would require more code to find the correct sentry file to read during logon
            // for the sake of simplicity, we'll just use "sentry.bin"

            int fileSize;
            byte[] sentryHash;
            using ( var fs = File.Open( "sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite ) )
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = ( int )fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using ( var sha = SHA1.Create() )
                {
                    sentryHash = sha.ComputeHash( fs );
                }
            }

            // inform the steam servers that we're accepting this sentry file
            user.SendMachineAuthResponse( new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            } );

            Program.Log("Done!");
        }

        private async Task GetLobbies()
        {
            if (!loggedIn)
            {
                await Program.Log("Steam isn't running yet");
                return;
            }
            
            vtolLobbies.Clear();
            jetborneLobbies.Clear();
            
            var vLobbyList = await matchmaking.GetLobbyList(Program.vtolID);
            var jLobbyList = await matchmaking.GetLobbyList(Program.jetborneID);
            
            vtolLobbies.AddRange(vLobbyList.Lobbies.Select(lobby => new VTOLLobby(lobby)));
            jetborneLobbies.AddRange(jLobbyList.Lobbies.Select(lobby => new JetborneLobby(lobby)));
        }
    }
}
