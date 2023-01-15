using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Scripting;
using System.Runtime;
using UnityEngine.Profiling;

namespace RMMBY.NeonLevelLoader
{
    public class Plugin : MelonMod
    {
        private bool inMenu;
        private bool buttonExists;

        public AssetBundle bundle;
        public MetadataLevel currentLevel;

        public int insight;

        public GameObject[] resultsButtons;

        public GameObject[] pauseButtons;

        public bool addedButton;

        public GameObject resultsLeader;
        public GameObject stagingLeader;
        public GameObject levelTitle;
        public GameObject levelEnvironment;

        public bool waitForTitle;

        public bool customLevelButtonDelegateSet;

        private int uploadShouldBeDisabled = 0;

        const long collectAfterAllocating = 16 * 1024 * 1024;

        private bool didSetup;

        private float garbageTimer = 30;

        public string LevelID()
        {
            string result = "";

            if(currentLevel != null)
            {
                result = string.Concat(currentLevel.Author, currentLevel.Title, currentLevel.Version).Replace(" ", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("*", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            }

            return result;
        }

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            GarbageCollector.GCMode = GarbageCollector.Mode.Manual;
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);

            if (sceneName == "CustomLevel")
            {
                LoggerInstance.Msg("Setting Up Level");

                LevelSetup.Setup();

                LoggerInstance.Msg("Setting Up Level");

                ToggleLeaderboardUpload(true);

                LoggerInstance.Msg("Setting Up Level");

                SetCustomLevelButtons();

                LoggerInstance.Msg("Setting Up Level");
            }
            else if (resultsButtons != null)
            {
                if (resultsButtons.Length > 0)
                {
                    resultsButtons[0].SetActive(true);
                    resultsButtons[1].SetActive(true);
                    resultsButtons[2].SetActive(false);

                    pauseButtons[0].SetActive(true);
                    pauseButtons[1].SetActive(true);
                    pauseButtons[2].SetActive(false);
                    pauseButtons[3].SetActive(false);
                }
            }

            if(sceneName == "CustomLevelMenu")
            {
                LoggerInstance.Msg("CLM Loaded");

                GameObject manager = GameObject.Instantiate(new GameObject());
                manager.AddComponent<MenuHandler>();

                CreateLevel.AddLevelCustomCampaign();

                ToggleLeaderboardUpload(false);

                inMenu = false;
            }
            else if (sceneName == "Menu")
            {
                inMenu = true;
                waitForTitle = false;

                UpdateCheck.UpdateForLevels();

                MenuFunction();
            }
            else inMenu = false;

            long mem = Profiler.GetMonoHeapSizeLong();
            if (mem > collectAfterAllocating)
            {
                GC.Collect(0);
                garbageTimer = 30;
            }
        }

        private void MenuFunction()
        {
            if (uploadShouldBeDisabled == 0)
            {
                switch (GameDataManager.powerPrefs.dontUploadToLeaderboard)
                {
                    case true:
                        uploadShouldBeDisabled = 2;
                        break;
                    case false:
                        uploadShouldBeDisabled = 1;
                        break;
                }
            }

            if (resultsButtons == null)
            {
                ButtonGenerators.CreateResults();
            }
            if (pauseButtons == null)
            {
                ButtonGenerators.CreatePause();
            }

            ToggleLeaderboardUpload(false);
        }

        private void GetLeaderboard()
        {
            if (resultsLeader == null)
            {
                try
                {
                    resultsLeader = GameObject.Find("Ingame Menu").transform.Find("Menu Holder").Find("Results Panel").Find("Leaderboards And LevelInfo").Find("Leaderboards").gameObject;
                }
                catch { }
            }

            if (stagingLeader == null)
            {
                try
                {
                    stagingLeader = GameObject.Find("Ingame Menu").transform.Find("Menu Holder").Find("Staging Panel").Find("Leaderboards And LevelInfo").Find("Leaderboards").gameObject;
                    levelTitle = stagingLeader.transform.parent.parent.Find("Level Title").gameObject;
                    levelEnvironment = levelTitle.transform.parent.Find("Level Environment").gameObject;
                }
                catch { }
            }
        }

        private void ToggleLeaderboardUpload(bool turnOn)
        {
            if (uploadShouldBeDisabled == 1)
            {
                if(turnOn) GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
                else GameDataManager.powerPrefs.dontUploadToLeaderboard = false;
            } else if (uploadShouldBeDisabled == 2) GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
        }

        public void SetCustomLevelButtons()
        {
            resultsButtons[0].SetActive(false);
            resultsButtons[1].SetActive(false);
            resultsButtons[2].SetActive(true);

            pauseButtons[0].SetActive(false);
            pauseButtons[1].SetActive(false);
            pauseButtons[2].SetActive(true);
            pauseButtons[3].SetActive(true);

            if (!customLevelButtonDelegateSet)
            {
                pauseButtons[3].GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
                pauseButtons[3].transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                pauseButtons[3].GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { ReloadLevel(); });
                pauseButtons[3].transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { ReloadLevel(); });

                customLevelButtonDelegateSet = true;
            }
        }

        public void DoPog()
        {
            if (waitForTitle) return;

            MelonLogger.Msg("Poggers! Settings should update now.");

            waitForTitle = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (garbageTimer > 0) garbageTimer -= Time.deltaTime;
            else
            {
                long mem = Profiler.GetMonoHeapSizeLong();
                if (mem > collectAfterAllocating) GC.Collect(0);
                garbageTimer = 30;
            }

            if (inMenu)
            {
                CreateMenuButton();
                GetLeaderboard();
                didSetup = true;
            }

            if (!didSetup) return;

            if (resultsButtons == null)
            {
                ButtonGenerators.CreateResults();
            }
            if (pauseButtons == null)
            {
                ButtonGenerators.CreatePause();
            }

            ModMenuHandler.onSettingsChanged += () => DoPog();

            if (Singleton<Game>.Instance.GetCurrentLevel().levelID == LevelID() && (pauseButtons[1].activeSelf || resultsButtons[1].activeSelf)) SetCustomLevelButtons();
        }

        public GameObject IndividualMenuButton(string text, string objName)
        {
            GameObject result = GameObject.Instantiate(GameObject.Find("Quit Button"));

            result.transform.SetParent(GameObject.Find("Title Buttons").transform);

            result.transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = text;

            result.name = objName;
            result.transform.localScale = Vector3.one;

            return result;
        }

        public void CreateMenuButton()
        {
            if(!GameObject.Find("CL Button") && !buttonExists)
            {
                if (!GameObject.Find("Quit Button")) return;

                GameObject clbutton = IndividualMenuButton("Custom Levels", "CL Button");

                clbutton.GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
                clbutton.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                clbutton.GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });
                clbutton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });

                clbutton.transform.SetSiblingIndex(3);

                MenuScreenTitle ms = GameObject.FindObjectOfType<MenuScreenTitle>();
                ms.buttonsToLoad.Add(clbutton.GetComponent<MenuButtonHolder>());

                GameObject disButton = IndividualMenuButton("Discord", "Discord Button");

                disButton.GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
                disButton.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                disButton.GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { Melon<Plugin>.Instance.LoadDiscord(); });
                disButton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { Melon<Plugin>.Instance.LoadDiscord(); });

                disButton.transform.SetSiblingIndex(5);

                ms.buttonsToLoad.Add(disButton.GetComponent<MenuButtonHolder>());

                GameObject modbutton = IndividualMenuButton("Mods", "Mod Button");

                modbutton.GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
                modbutton.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                modbutton.GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { LoadMenu(); });
                modbutton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { LoadMenu(); });

                modbutton.transform.SetSiblingIndex(4);

                List<MenuButtonHolder> list = new List<MenuButtonHolder>();

                for (int i = 0; i < 3; i++)
                {
                    list.Add(ms.buttonsToLoad[i]);
                }

                list.Add(ms.buttonsToLoad[5]);
                list.Add(modbutton.GetComponent<MenuButtonHolder>());
                list.Add(ms.buttonsToLoad[3]);
                list.Add(ms.buttonsToLoad[6]);
                list.Add(ms.buttonsToLoad[4]);

                ms.buttonsToLoad = list;

                ms.LoadButtons();

                buttonExists = true;
                inMenu = false;
            } else if (buttonExists)
            {
                inMenu = false;

                GameObject.FindObjectOfType<MenuScreenTitle>().LoadButtons();
            }
        }

        public void LoadMenu()
        {
            LoadModMenu.CheckForBundle("CustomLevelMenu");
        }

        public void ReloadLevel()
        {
            if (SceneManager.GetActiveScene().name == "CustomLevel")
            {
                bundle.Unload(true);

                string path = currentLevel.Location;
                bundle = AssetBundle.LoadFromFile(Path.Combine(path, currentLevel.AssetBundleName));
                Singleton<Game>.Instance.PlayLevel(string.Concat(currentLevel.Author, currentLevel.Title, currentLevel.Version).Replace(" ", ""), true);
            }
        }

        public void LoadDiscord()
        {
            System.Diagnostics.Process.Start("https://discord.gg/SFnWweK8r9");
        }

        [HarmonyPatch(typeof(Leaderboards), "SetLevel", new Type[] { typeof(LevelData), typeof(bool), typeof(bool) })]
        private static class LeaderboardPatch
        {
            private static bool Prefix()
            {
                if (Singleton<Game>.Instance.GetCurrentLevel().levelID == Melon<Plugin>.Instance.LevelID())
                {
                    Melon<Plugin>.Instance.resultsLeader.SetActive(false);
                    Melon<Plugin>.Instance.stagingLeader.SetActive(false);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(MenuButtonHolder), "OnEnable", null)]
        private static class ResultsPatch
        {
            private static void Prefix()
            {
                if (SceneManager.GetActiveScene().name == "CustomLevel")
                {
                    Melon<Plugin>.Instance.resultsButtons[0].SetActive(false);
                    Melon<Plugin>.Instance.resultsButtons[1].SetActive(false);
                    Melon<Plugin>.Instance.resultsButtons[2].SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(MenuScreenPause), "OnSetVisible", null)]
        private static class PausePatch
        {
            private static void Postfix()
            {
                if (SceneManager.GetActiveScene().name == "CustomLevel")
                {
                    Melon<Plugin>.Instance.pauseButtons[0].SetActive(false);
                    Melon<Plugin>.Instance.pauseButtons[1].SetActive(false);
                    Melon<Plugin>.Instance.pauseButtons[2].SetActive(true);
                    Melon<Plugin>.Instance.pauseButtons[3].SetActive(true);

                    GameObject.FindObjectOfType<MenuScreenPause>().LoadButtons();
                }
            }
        }

        [HarmonyPatch(typeof(MenuScreenStaging), "OnSetVisible", null)]
        private static class LevelTextPatch
        {
            private static void Postfix()
            {
                if (SceneManager.GetActiveScene().name == "CustomLevel")
                {
                    Melon<Plugin>.Instance.levelTitle.GetComponent<TMP_Text>().text = Melon<Plugin>.Instance.currentLevel.Title;
                    Melon<Plugin>.Instance.levelEnvironment.GetComponent<TMP_Text>().text = "DISTRICT: " + LevelSetup.GetDistrictName(Melon<Plugin>.Instance.currentLevel.EnvironmentType);
                }
            }
        }

        [HarmonyPatch(typeof(MenuScreenTitle), "OnSetVisible", new Type[] { typeof(bool) })]
        private static class TitlePatch
        {
            private static void Postfix()
            {
                if (!GameObject.Find("CL Button"))
                {

                }
            }
        }
    }
}
