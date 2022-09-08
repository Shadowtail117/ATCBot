using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace ATCBot
{
    /// <summary>
    /// Class for containing the current version of the bot and verifying remote version.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// The local version of the bot obtained from version.txt.
        /// </summary>
        public static string LocalVersion { get; } = "1.4.2";

        /// <summary>
        /// The remote version on the repository.
        /// </summary>
        public static string RemoteVersion { get; private set; }

        private static readonly string url = "https://gist.githubusercontent.com/Shadowtail117/507f77becaa1ea91a0caf6d1eca2e0ec/raw";

        /// <summary>
        /// Retrieve the remote version stored on the repository and verify it matches the local version.
        /// </summary>
        /// <returns>Whether or not the local version matches the remote version.</returns>
        public static async Task<bool> CheckVersion()
        {
            HttpClient client = new();

            try
            {
                RemoteVersion = await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Log.LogError("Could not get remote version!", e, "Version Checker", true);
                RemoteVersion = "ERR";
            }
            return LocalVersion == RemoteVersion;
        }
    }
}
