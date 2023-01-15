using MelonLoader;
using UnityEngine;
using RMMBY.Editable;
using RMMBY.Helpers;
using System;
using System.Runtime.InteropServices;
using RMMBY.GameBanana;
using UnityEngine.SceneManagement;

namespace RMMBY
{
    public class Plugin : MelonMod
    {
        private bool inScene;
        private EnabledMods em;
        private InputHandler inputHandler;

        private bool holdConsoleToggle;
        private bool consoleHidden;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private string infoText = "";
        private int infoType;
        private bool inInfo;

        private bool inMenu;
        private bool discordSet;

        public void LoadInfo(string message, int type)
        {
            infoText = message;
            infoType = type;

            LoadModMenu.CheckForBundle("RMMBYInfo");
        }

        public void LoadInfo(int type)
        {
            switch (type)
            {
                case 0:
                    LoadModMenu.CheckForBundle("RMMBYInfo");
                    break;
            }
        }

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            RegistryHelper.ValidateRegistry();

            //ShowWindow(GetConsoleWindow(), 0);
            //consoleHidden = true;

            DiscordFunctions.gameStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);

            DiscordFunctions.OnSceneChanged();

            if (em == null)
            {
                em = new GameObject().AddComponent<EnabledMods>();
                em.name = "RMMBY";
                em.gameObject.AddComponent<InputHandler>();
                inputHandler = em.gameObject.GetComponent<InputHandler>();
            }

            if (sceneName == ListenToLoadMenu.sceneToListen)
            {
                inScene = true;
                ListenToLoadMenu.OnSceneStart();
            }
            else
            {
                inScene = false;
            }

            if (sceneName == "RMMBYModMenu")
            {
                GameObject go = new GameObject();
                go.AddComponent<ModMenuHandler>();

                inputHandler.OnSceneLoaded();
                inputHandler.active = true;
            } else if (sceneName == "RMMBYInfo")
            {
                GameObject go = new GameObject();
                go.AddComponent<InfoMenuHandler>();

                inputHandler.OnSceneLoaded();
                inputHandler.active = true;

                inInfo = true;
            }
            else
            {
                inputHandler.active = false;
            }

            if(sceneName == "Menu")
            {
                if (!ListenToLoadMenu.setMenuFunction) return;

                ModUpdater.CheckForUpdates("Mods", 0);

                ListenToLoadMenu.UpdateForMods();

                inMenu = true;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (inInfo) SetInfo();

            if (DiscordFunctions.useRichPresence && DiscordFunctions.discord != null) DiscordFunctions.OnUpdate();

            if (inScene && ListenToLoadMenu.runOnUpdate)
            {
                ListenToLoadMenu.OnSceneUpdate();
            }

            if(!inputHandler.toggleConsole) {
                holdConsoleToggle = false;
            }

            if (inputHandler.toggleConsole && !holdConsoleToggle)
            {
                switch (consoleHidden)
                {
                    case true:
                        ShowWindow(GetConsoleWindow(), 5);
                        consoleHidden = false;
                        break;
                    case false:
                        ShowWindow(GetConsoleWindow(), 0);
                        consoleHidden = true;
                        break;
                }
                
                holdConsoleToggle = true;
            }
        }

        public void SetInfo()
        {
            if (!GameObject.FindObjectOfType<InfoMenuHandler>()) return;

            switch (infoType)
            {
                case 0:
                    GameObject.FindObjectOfType<InfoMenuHandler>().SetupRestart(infoText);
                    break;
            }

            inInfo = false;
        }
    }
}
