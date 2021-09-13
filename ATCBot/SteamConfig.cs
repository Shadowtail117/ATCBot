using System.IO;
using Newtonsoft.Json;

namespace ATCBot
{
    public class SteamConfig
    {
        public static SteamConfig Config;
        public string SteamUserName;
        public string SteamPassword;
        public string F2ACode;
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
        
        
        public static SteamConfig Load()
        {
            if (!Directory.Exists(saveDirectory) || !File.Exists(saveFile))
            {
                Program.Log($"steam.json doesn't exist. Creating one!");
                Save(new SteamConfig());
            }
            
            Config = JsonConvert.DeserializeObject<SteamConfig>(File.ReadAllText(saveFile));
            return Config;
        }
    }
}