using System;
using System.Threading;

namespace ATCBot
{
    /// <summary>
    /// Watches for lobby updates and alerts if they fall behind or fail entirely.
    /// </summary>
    internal static class Watchdog
    {
        private static Program program;

        private static double waitTime;

        public static DateTime lastUpdate;

        private static Timer timer;

        private static int skippedBeats = 0;

        public static void Start()
        {
            program = Program.program;
            waitTime = Program.config.delay;
            timer = new(CheckStatus, null, TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(waitTime));
        }

        private static bool triedRestart;

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

            if (DateTime.Now - lastUpdate > TimeSpan.FromSeconds(waitTime) * 2) //We wait 2 full cycles to negate any possible latency issues that would cause it to be counted as a miss
            {
                skippedBeats++;
                switch (skippedBeats)
                {
                    case int n when n < 5:
                        Log.LogWarning($"Watchdog detected a skipped heartbeat! This is the {AddOrdinal(n)} time in a row!", "Watchdog");
                        break;
                    default:
                        {
                            if (triedRestart)
                            {
                                Log.LogCritical($"Watchdog has detected a skipped heartbeat for the 5th time in a row and reviving did not work! Pulling the plug!", source: "Watchdog");
                                Environment.Exit(2);
                                break;
                            }
                            else
                            {
                                Log.LogError($"Watchdog has detected a skipped heartbeat for the 5th time in a row! Trying to revive the query timer...", source: "Watchdog", announce: true);
                                Program.lobbyHandler.client.Disconnect();
                                Program.lobbyHandler = new(program);
                                _ = Program.lobbyHandler.QueryTimer(LobbyHandler.queryToken.Token);
                                Program.lobbyHandler.ResetQueryTimer();
                                skippedBeats = 0;
                                triedRestart = true;
                                break;
                            }
                        }
                }
            }
            else
            {
                Log.LogDebug("Watchdog reports an acceptable heartbeat.", "Watchdog");
                skippedBeats = 0;
                if(triedRestart)
                {
                    triedRestart = false;
                    Log.LogInfo("Watchdog seems to have successfully defibrillated the queries!", "Watchdog", true);
                }
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
