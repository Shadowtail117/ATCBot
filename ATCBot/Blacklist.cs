using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCBot
{
    internal static class Blacklist
    {
        public static List<long> blacklist = new();

        private static readonly string directory = Directory.GetCurrentDirectory();
        private static readonly string saveDirectory = Path.Combine(directory, @"Config");
        private static readonly string saveFile = Path.Combine(saveDirectory, @"blacklist.txt");

        public static void Save()
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            StringBuilder sb = new();
            foreach (long l in blacklist) sb.AppendLine(l.ToString());
            File.WriteAllText(saveFile, sb.ToString());
        }

        public static bool Load()
        {
            if (!Directory.Exists(saveDirectory) || !File.Exists(saveFile))
            {
                Log.LogInfo("blacklist.txt does not existing, creating it...");
                Save();
                return false;
            }

            string[] file = File.ReadAllLines(saveFile);
            foreach(string s in file)
            {
                bool successful = long.TryParse(s, out long item);
                if (!successful)
                {
                    Log.LogWarning($"Could not read item \"{s}\" in blacklist file!");
                    continue;
                }
                blacklist.Add(item);
            }

            return true;
        }
    }
}
