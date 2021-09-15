using System.IO;
using Newtonsoft.Json;

namespace ATCBot
{
    public class SteamConfig
    {
        /// <summary>
        /// A global steam config to use for this instance of the bot.
        /// </summary>
        public static SteamConfig Config;
        
        /// <summary>
        /// The username of the steam account which will be logged into.
        /// </summary>
        public string SteamUserName;
        
        /// <summary>
        /// The password for the steam account which will be logged into.
        /// </summary>
        public string SteamPassword;
        
        /// <summary>
        /// The two factor authentication code to be used if the steam account has 2FA enabled.
        /// </summary>
        public string TwoFactorAuthCode;
        
        /// <summary>
        /// The steam guard code to be used if the steam account has it enabled.
        /// </summary>
        public string AuthCode;
        
        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Config");
        private static readonly string saveFile = Path.Combine(saveDirectory, @"steam.json");

        private static void Save(SteamConfig config)
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            
            File.WriteAllText(saveFile, JsonConvert.SerializeObject(config));
            Console.WriteLine($"Saved config to {saveFile}");
        }
        
        
        public static bool Load()
        {
            if (!Directory.Exists(saveDirectory) || !File.Exists(saveFile))
            {
                Program.Log($"steam.json doesn't exist. Creating one! " +
                            $"Please add your steam details int steam.json");
                Save(new SteamConfig());
                return false;
            }
            
            Config = JsonConvert.DeserializeObject<SteamConfig>(File.ReadAllText(saveFile));

            if (Config.SteamUserName == null || Config.SteamPassword == null)
            {
                Console.WriteLine($"{saveFile} has no steam login details. Please put your login details in there.");
                return false;
            }
            
            return true;
        }
    }
}