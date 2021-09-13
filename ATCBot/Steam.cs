using System;
using System.IO;
using System.Security.Cryptography;
using SteamKit2;

namespace ATCBot
{
    public static class Steam
    {
        public static bool Running;
        
        private static SteamClient client;
        private static CallbackManager manager;
        private static SteamUser user;
        
        public static void SetupSteam()
        {
            SteamConfig.Load();
            client = new SteamClient();
            manager = new CallbackManager(client);
            user = client.GetHandler<SteamUser>();

            SetupCallbacks();

            Program.Log("Connecting to steam");
            Running = true;
            client.Connect();

            while (Running)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private static void SetupCallbacks()
        {
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);
        }
        
        private static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Program.Log($"Connected to steam. Logging into {SteamConfig.Config.SteamUserName}");

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
                TwoFactorCode = SteamConfig.Config.F2ACode,
                SentryFileHash = sentryHash
            });
        }
        
        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Program.Log("Disconnected from Steam");
            Running = false;
        }
        
        private static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            bool hasSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool hasF2A = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            if (hasSteamGuard)
            {
                Program.Log($"Emailed Auth Code was invalid. Please update the steam.json and try again");
                return;
            }
            
            if (hasF2A)
            {
                Program.Log("F2A code was invalid. Please update the steam.json and try again");
                return;
            }
            
            if (callback.Result != EResult.OK)
            {
                Running = false;
                Program.Log($"Failed to log into steam. {callback.Result} {callback.ExtendedResult}");
                return;
            }

            Program.Log("Logged into steam account");
            Running = true;
        }

        private static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
           
        }
        
        private static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
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

    }
}