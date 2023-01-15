using HarmonyLib;
using UnityEngine;
using System;

using TMPro;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;
using RMMBY.GameBanana;
using MelonLoader;

namespace RMMBY.Editable
{
    public class ListenToLoadMenu
    {
        public static string sceneToListen = "Menu";
        public static bool runOnUpdate = true;
        public static string sceneToReturnTo = "Menu";

        public static bool setMenuFunction = false;

        public static void OnSceneStart()
        {
        }

        public static void OnSceneUpdate()
        {
            if (GameObject.Find("Mod Button"))
            {
                if (setMenuFunction) return;

                ModUpdater.CheckForUpdates("Mods", 0);

                GameObject modbutton = GameObject.Find("Mod Button");
                modbutton.GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
                modbutton.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                modbutton.GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { LoadMenu(); });
                modbutton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { LoadMenu(); });

                setMenuFunction = true;

                UpdateForMods();
            }
        }

        public static void LoadMenu()
        {
            LoadModMenu.CheckForBundle("RMMBYModMenu");
        }

        public static void UpdateForMods()
        {
            GameObject modbutton = GameObject.Find("Mod Button");

            Melon<Plugin>.Logger.Msg("Mods With Updates = " + ModUpdater.modsWithUpdates[0].Count);

            ColorBlock cb = modbutton.transform.Find("Button").GetComponent<Button>().colors;
            if (ModUpdater.modsWithUpdates[0].Count > 0)
            {
                cb.normalColor = Color.green;

                modbutton.transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Updates - Mods";
            }
            else
            {
                cb.normalColor = Color.white;

                modbutton.transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Mods";
            }
            modbutton.transform.Find("Button").GetComponent<Button>().colors = cb;

            modbutton.transform.Find("Button").GetComponent<Button>().OnPointerExit(null);

            GameObject.FindObjectOfType<MenuScreenTitle>().LoadButtons();
        }
    }
}