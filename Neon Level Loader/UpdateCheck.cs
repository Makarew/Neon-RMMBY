using MelonLoader;
using RMMBY.GameBanana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RMMBY.NeonLevelLoader
{
    internal class UpdateCheck
    {
        public static void UpdateForLevels()
        {
            ModUpdater.CheckForUpdates("Levels", 0);

            GameObject button = GameObject.Find("CL Button");

            Melon<Plugin>.Logger.Msg("Levels With Updates = " + ModUpdater.modsWithUpdates[1].Count);

            ColorBlock cb = button.transform.Find("Button").GetComponent<Button>().colors;
            if (ModUpdater.modsWithUpdates[1].Count > 0 )
            {
                cb.normalColor = Color.green;
                button.transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Updates - Custom Levels";
            } else
            {
                cb.normalColor = Color.white;
                button.transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Custom Levels";
            }
            button.transform.Find("Button").GetComponent<Button>().colors = cb;

            button.transform.Find("Button").GetComponent<Button>().OnPointerExit(null);

            GameObject.FindObjectOfType<MenuScreenTitle>().LoadButtons();
        }
    }
}
