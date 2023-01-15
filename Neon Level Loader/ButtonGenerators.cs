using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;

namespace RMMBY.NeonLevelLoader
{
    internal class ButtonGenerators
    {
        public static void CreatePause()
        {
            List<GameObject> buttons = new List<GameObject>();

            buttons.Add(GameObject.Find("Ingame Menu").transform.Find("Menu Holder").Find("Pause Menu").Find("Pause Menu Holder").Find("Pause Buttons").Find("Job Archive Button Holder").gameObject);
            buttons.Add(buttons[0].transform.parent.Find("Return to HUB Button Holder").gameObject);

            buttons.Add(GameObject.Instantiate(buttons[0]));

            buttons[2].transform.SetParent(buttons[0].transform.parent);

            buttons[2].transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Return To Level Select";

            buttons[2].name = "Level Select Button Holder";
            buttons[2].transform.localScale = Vector3.one;

            buttons[2].GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
            buttons[2].transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            buttons[2].GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });
            buttons[2].transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });
            buttons[2].transform.SetSiblingIndex(9);

            List<MenuButtonHolder> list = new List<MenuButtonHolder>();
            for (int i = 0; i < 9; i++)
            {
                list.Add(buttons[2].transform.parent.parent.parent.GetComponent<MenuScreenPause>().buttonsToLoad[i]);
            }

            buttons[2].SetActive(false);

            buttons.Add(GameObject.Instantiate(buttons[0]));

            buttons[3].transform.SetParent(buttons[0].transform.parent);

            buttons[3].transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Reload Level";

            buttons[3].name = "Reload Level Button Holder";
            buttons[3].transform.localScale = Vector3.one;

            buttons[3].GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
            buttons[3].transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            buttons[3].GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { Melon<Plugin>.Instance.ReloadLevel(); });
            buttons[3].transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { Melon<Plugin>.Instance.ReloadLevel(); });
            buttons[3].transform.SetSiblingIndex(9);

            buttons[3].SetActive(false);

            list.Add(buttons[3].GetComponent<MenuButtonHolder>());
            list.Add(buttons[2].GetComponent<MenuButtonHolder>());
            list.Add(buttons[2].transform.parent.parent.parent.GetComponent<MenuScreenPause>().buttonsToLoad[9]);

            buttons[2].transform.parent.parent.parent.GetComponent<MenuScreenPause>().buttonsToLoad = list;

            Melon<Plugin>.Instance.pauseButtons = buttons.ToArray();
        }

        public static void CreateResults()
        {
            List<GameObject> buttons = new List<GameObject>();

            buttons.Add(GameObject.Find("Menu Holder").transform.Find("Results Panel").Find("Results Buttons").Find("Button Hub").gameObject);
            buttons.Add(GameObject.Find("Menu Holder").transform.Find("Results Panel").Find("Results Buttons").Find("Button Job Archive").gameObject);

            buttons.Add(GameObject.Instantiate(buttons[0]));

            buttons[2].transform.SetParent(buttons[0].transform.parent);

            buttons[2].transform.Find("Button").Find("Text").GetComponent<TMP_Text>().text = "Return To Level Select";

            buttons[2].name = "Button Level Select";
            buttons[2].transform.localScale = Vector3.one;

            buttons[2].GetComponent<MenuButtonHolder>().onClickEvent.RemoveAllListeners();
            buttons[2].transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            buttons[2].GetComponent<MenuButtonHolder>().onClickEvent.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });
            buttons[2].transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate { Melon<Plugin>.Instance.LoadMenu(); });

            buttons[2].transform.parent.parent.GetComponent<MenuScreenResults>().buttonsToLoad.Add(buttons[2].GetComponent<MenuButtonHolder>());

            buttons[2].SetActive(false);

            Melon<Plugin>.Instance.resultsButtons = buttons.ToArray();
        }
    }
}
