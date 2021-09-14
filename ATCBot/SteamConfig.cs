using System.IO;
using Newtonsoft.Json;

namespace ATCBot
{
    public class SteamConfig
    {
        public static SteamConfig Config;
        public string SteamUserName;
        public string SteamPassword;
        public string TwoFactorAuthCode;
        public string AuthCode;
        
        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Config");
        private static readonly string saveFile = Path.Combine(saveDirectory, @"steam.json");

        private static void Save(SteamConfig config)
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            
            File.WriteAllText(saveFile, JsonConvert.SerializeObject(config));
            Program.Log($"Saved config to {saveFile}");
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
                Program.Log($"{saveFile} has no steam login details. Please put your login details in there.");
                return false;
            }
            
            return true;
        }
    }
}