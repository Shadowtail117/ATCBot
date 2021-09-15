using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;

namespace ATCBot
{
    /// <summary>
    /// Class for containing the current version of the bot and verifying remote version.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// The local version of the bot.
        /// </summary>
        public static string LocalVersion { get; } = "1.1.0";

        /// <summary>
        /// The remote version on the repository.
        /// </summary>
        public static string RemoteVersion { get; private set; }

        private static readonly string url = "https://raw.githubusercontent.com/Shadowtail117/ATCBot/versioncheck/ATCBot/version.txt";

        /// <summary>
        /// Retrieve the remote version stored on the repository and verify it matches the local version.
        /// </summary>
        /// <returns>Whether or not the local version matches the remote version.</returns>
        public static async Task<bool> CheckVersion()
        {
            HttpClient client = new HttpClient();

            try
            {
                RemoteVersion = await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Warning, "Version Checker", "Could not get remote version!", e));
                RemoteVersion = "ERR";
            }
            return LocalVersion == RemoteVersion;
        } 
    }
}
