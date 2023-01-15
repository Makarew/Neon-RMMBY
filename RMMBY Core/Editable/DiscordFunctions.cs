using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using RMMBY.Discord;
using System.Reflection;
using MelonLoader;

namespace RMMBY.Editable
{
    internal class DiscordFunctions
    {
        /*
         * This class is for interacting with the Discord Game SDK. The SDK library is not included with RMMBY. Download the Discord Game SDK from Discord's website. 
         * Locate the x86_64 "discord_game_sdk.dll" and place it in "Plugins/x86_64" in your game's data folder.
         * SDK information is not included with RMMBY. Use the Discord Developer site to learn more about the Discord Game SDK's features.
         */

        //Change this if you don't want Discord Rich Presence
        public static bool useRichPresence = true;

        //Don't change these
        public static Discord.Discord discord;
        private static Activity activity;
        private static bool initialized;

        //Discord limits rich presence updates to 5 every 20 seconds. To match this, RMMBY updates every 4 seconds by default. Change updateRate for different timing.
        private static float updateRate = 4;
        private static float timeToNextUpdate = 0;

        //The time the player started playing. Used for displaying total game time elapsed in Discord.
        public static long gameStartTime;

        //Game specific variables
        private static bool inLevel;
        private static string lastLevel;
        private static int resets;
        private static bool setForStaging;

        public static void Initialize()
        {
            if (!useRichPresence || initialized) return;

            //Change application ID to your application ID
            discord = new Discord.Discord(1061482280872316949, (System.UInt64)Discord.CreateFlags.Default);
            //This activity isn't required. It's just a failsafe in the event UpdateRichPresence() is called before an activity is actually set.
            activity = new Discord.Activity
            {
                Details = "Just Started Playing",
                Assets =
                {
                    LargeImage = "rmmby_bee",
                    SmallImage = "rmmby_bee"
                }
            };

            initialized = true;
        }

        public static void OnSceneChanged()
        {
            if (!useRichPresence) return;
            if (discord == null) Initialize();

            if (timeToNextUpdate <= 0)
            {
                if (SceneManager.GetActiveScene().name == "IntroCards" || SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "RMMBYModMenu" || SceneManager.GetActiveScene().name == "CustomLevelMenu")
                {
                    activity = new Discord.Activity
                    {
                        State = "",
                        Details = "In Menu",
                        Assets =
                {
                    LargeImage = "neonwhite",
                    SmallImage = "rmmby_bee"
                }
                    };

                    UpdateRichPresence(true);
                    resets = 0;
                    timeToNextUpdate = updateRate;
                    lastLevel = "";
                    return;
                }
            }
            if (Singleton<Game>.Instance.GetCurrentLevel() == null) return;

            LevelData leveldata = Singleton<Game>.Instance.GetCurrentLevel();

            if (timeToNextUpdate <= 0)
            {
                if (leveldata.levelDisplayName == "Heaven Plaza")
                {
                    activity = new Discord.Activity
                    {
                        State = "",
                        Details = "In Menu",
                        Assets =
                {
                    LargeImage = "neonwhite",
                    SmallImage = "rmmby_bee"
                }
                    };

                    UpdateRichPresence(true);
                    resets = 0;
                    timeToNextUpdate = updateRate;
                    lastLevel = "";
                    return;
                }
            }

            if (SceneManager.GetActiveScene().name == leveldata.levelScene.SceneName)
            {
                FieldInfo field = typeof(Game).GetField("_currentPlaythrough", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlaythrough lp = (LevelPlaythrough)field.GetValue(Singleton<Game>.Instance);

                if (lp.timer == -1 && !setForStaging)
                {
                    inLevel = false;

                    string image = "neonwhite";
                    if (SceneManager.GetActiveScene().name != "CustomLevel") image = leveldata.levelDisplayName.ToLower().Replace(" ", "").Replace("'", "");

                    activity = new Discord.Activity
                    {
                        State = "Last Run: " + Game.GetTimerFormatted(GameDataManager.GetLevelStats(leveldata.levelID).GetTimeLastMicroseconds()) + " | Resets: " + resets,
                        Details = leveldata.levelDisplayName + " | PB: " + Game.GetTimerFormatted(GameDataManager.GetLevelStats(leveldata.levelID).GetTimeBestMicroseconds()),
                        Assets =
                        {
                            LargeImage = image,
                            SmallImage = "rmmby_bee"
                        }
                    };

                    UpdateRichPresence(false);
                    setForStaging = true;
                    return;
                }
            }
            else resets = 0;
        }

        public static void OnUpdate()
        {
            //Don't delete
            if (!useRichPresence || discord == null) return;
            discord.RunCallbacks();

            //Don't delete. This is the updateRate timer.
            if (timeToNextUpdate > 0)
            {
                timeToNextUpdate -= Time.deltaTime;
            }

            if (Singleton<Game>.Instance.GetCurrentLevel() == null) return;

            //Neon White specific code. Use this section for updates that are set up to not run in time with updateRate
            LevelData leveldata = Singleton<Game>.Instance.GetCurrentLevel();

                FieldInfo field = typeof(Game).GetField("_currentPlaythrough", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlaythrough lp = (LevelPlaythrough)field.GetValue(Singleton<Game>.Instance);
                if (setForStaging && lp.timer == -1) return;
                else if (inLevel) return;
                else if (!setForStaging) return;

                if (lastLevel != leveldata.levelID)
                {
                    lastLevel = leveldata.levelID;
                    resets = 0;
                }
                else resets++;

            string image = "neonwhite";
            if (SceneManager.GetActiveScene().name != "CustomLevel") image = leveldata.levelDisplayName.ToLower().Replace(" ", "").Replace("'", "");

            activity = new Discord.Activity
                {
                    State = "Last Run: " + Game.GetTimerFormatted(GameDataManager.GetLevelStats(leveldata.levelID).GetTimeLastMicroseconds()) + " | Resets: " + resets,
                    Details = leveldata.levelDisplayName + " | PB: " + Game.GetTimerFormatted(GameDataManager.GetLevelStats(leveldata.levelID).GetTimeBestMicroseconds()),
                    Assets =
                    {
                        LargeImage = image,
                        SmallImage = "rmmby_bee"
                    },
                    Instance = true
                };

                activity.Timestamps = new ActivityTimestamps
                {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };

                UpdateRichPresence(false);
                inLevel = true;
                setForStaging = false;
                return;
        }

        //Sends rich presence update to your Discord client
        private static void UpdateRichPresence(bool addTotalGameTime)
        {
            if (addTotalGameTime)
            {
                activity.Timestamps = new ActivityTimestamps
                {
                    Start = gameStartTime,
                };
            }

            var activityManager = discord.GetActivityManager();

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Debug.LogError("Everything is fine!");
                }
            });
        }
    }
}
