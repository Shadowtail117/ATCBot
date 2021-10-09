using System;
using System.Threading;

namespace ATCBot
{
    /// <summary>
    /// Watches for lobby updates and alerts if they fall behind or fail entirely.
    /// </summary>
    internal static class Watchdog
    {
        private static double waitTime;

        public static DateTime lastUpdate;

        private static Timer timer;

        private static int skippedBeats = 0;

        public static void Start()
        {
            waitTime = Program.config.delay;
            timer = new(CheckStatus, null, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(waitTime));
        }

        private static void CheckStatus(object info)
        {
            if (waitTime != Program.config.delay)
            {
                waitTime = Program.config.delay;
                timer.Change(TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(waitTime));
            }

            if (!Program.updating || !LobbyHandler.loggedIn || !LobbyHandler.triedLoggingIn)
            {
                Log.LogDebug("Watchdog reports we are either not updating or not logged in.", "Watchdog");
                return;
            }

            if (DateTime.Now - lastUpdate > TimeSpan.FromSeconds(waitTime))
            {
                skippedBeats++;
                switch (skippedBeats)
                {
                    case int n when n < 4:
                        Log.LogWarning($"Watchdog detected a skipped heartbeat! This is the {AddOrdinal(n)} time in a row!", "Watchdog");
                        break;
                    default:
                        Log.LogCritical($"Watchdog has detected a skipped heartbeat for the 5th time in a row! Pulling the plug!", source: "Watchdog");
                        Environment.Exit(2);
                        break;
                }
            }
            else
            {
                Log.LogDebug("Watchdog reports an acceptable heartbeat.", "Watchdog");
                skippedBeats = 0;
            }

            string AddOrdinal(int n)
            {
                return n switch
                {
                    int u when u % 100 is 11 or 12 or 13 => u + "th",
                    int u when u % 10 is 1 => u + "st",
                    int u when u % 10 is 2 => u + "nd",
                    int u when u % 10 is 3 => u + "rd",
                    _ => n + "th",
                };
            }
        }
    }
}
