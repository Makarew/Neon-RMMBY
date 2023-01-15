using RMMBY.Editable;
using UnityEngine;
using System.IO;
using MelonLoader;
using RMMBY.Helpers;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RMMBY.GameBanana;

namespace RMMBY.NeonLevelLoader
{
    internal class MenuHandler : MonoBehaviour
    {
        private bool down;
        private bool up;
        private bool left;
        private bool right;
        private bool select;
        private bool selectLock = true;
        private bool back;
        private bool backLock = true;
        private bool refresh;
        private bool refreshLock = true;
        private bool update;
        private bool updateLock = true;

        Canvas modmenu;
        private float scaleFactor = 1;

        private InputHandler inputHandler;

        private string levelDirectory;

        private bool error;

        private int buttonDir;
        private Vector2 buttonStart;
        private float buttonYDif;
        private float buttonXDif;
        private float currentY;
        private float currentX;
        private float buttonPeak;
        private float buttonValley;

        private GameObject buttonHolder;
        private List<GameObject> buttons = new List<GameObject>();
        public GameObject selectedObject;

        public GameObject buttonPrefab;

        private bool levelsLoaded = false;

        private int currentButtonSelect = 0;

        public static List<MetadataLevel> Metadata = new List<MetadataLevel>();
        public Text[] levelText;
        public Image medal;
        public Sprite[] medals;

        private GameObject titlePanel;

        public void Start()
        {
            Time.timeScale = 1;

            inputHandler = FindObjectOfType<InputHandler>();
            inputHandler.active = true;
            levelDirectory = Path.Combine(MelonHandler.ModsDirectory, "RMMBY/Levels");
            SetVariables();

            float referenceResolution = 0;
            try
            {
                referenceResolution = float.Parse(DataReader.ReadData("ModMenuScreenResolutionReference"));
            }
            catch { DataError(); }

            if (error) return;

            LoadLevels();
            currentY = buttonStart.y;
            currentX = buttonStart.x;
            buttonPrefab = GameObject.Find("ButtonPrefab");
            buttonHolder = GameObject.Find("ModSelectionMenu").transform.Find("Mask").Find("Buttons").gameObject;
            CreateButtons();

            modmenu = GameObject.Find("ModMenu").GetComponent<Canvas>();
            float screenHeight = Screen.height;
            scaleFactor = screenHeight / referenceResolution;
            modmenu.scaleFactor = scaleFactor;

            titlePanel = GameObject.FindObjectOfType<MainMenu>().transform.Find("Canvas").Find("Main Menu").Find("Panel").Find("Title Panel").gameObject;
            titlePanel.SetActive(false);

            GameObject.Find("Audio").GetComponent<Audio>().StopAll(0);
            GameObject.Find("Audio").GetComponent<Audio>().PlayMusic("MUSIC_STORY_TITLE");
        }

        public void Update()
        {
            if (inputHandler.back)
            {
                if (!backLock)
                {
                    back = true;
                    backLock = true;
                }
                else
                {
                    back = false;
                }
            }
            else
            {
                backLock = false;
                back = false;
            }

            if (!levelsLoaded)
            {
                if (back)
                {
                    titlePanel.SetActive(true);
                    SceneManager.LoadScene(ListenToLoadMenu.sceneToReturnTo);
                }
                return;
            }

            if (inputHandler.select)
            {
                if (!selectLock)
                {
                    select = true;
                    selectLock = true;
                }
                else
                {
                    select = false;
                }
            }
            else
            {
                selectLock = false;
                select = false;
            }

            if (inputHandler.update)
            {
                if (!updateLock)
                {
                    update = true;
                    updateLock = true;
                }
                else
                { update = false; }
            }
            else
            {
                updateLock = false;
                update = false;
            }

            if (inputHandler.refresh)
            {
                if (!refreshLock)
                {
                    refresh = true;
                    refreshLock = true;
                }
                else { refresh = false; }
            }
            else
            {
                refreshLock = false;
                refresh = false;
            }

            Inputs();

            SetText(buttons[currentButtonSelect]);

            SetMenuItemsPosition();
        }

        private void LoadLevel()
        {
            if (Melon<Plugin>.Instance.bundle != null)
            {
                Melon<Plugin>.Instance.bundle.Unload(true);
                Melon<Plugin>.Instance.bundle = null;
            }

            MetadataLevel meta = Metadata[currentButtonSelect];

            string path = meta.Location;
            Melon<Plugin>.Instance.bundle = AssetBundle.LoadFromFile(Path.Combine(path, meta.AssetBundleName));

            Melon<Plugin>.Logger.Msg("Make Level Data");
            CreateLevel.LoadNewLevelData(path);

            Melon<Plugin>.Logger.Msg("Load Level: " + path);
            Singleton<Game>.Instance.PlayLevel(string.Concat(meta.Author, meta.Title, meta.Version).Replace(" ", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("*", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", ""), true);
        }

        private void Inputs()
        {
            switch (buttonDir)
            {
                case 0:
                    if (inputHandler.down)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.up)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
                case 2:
                    if (inputHandler.down || inputHandler.right)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.up || inputHandler.left)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
                case 3:
                    if (inputHandler.down || inputHandler.left)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.up || inputHandler.right)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
                case 1:
                    if (inputHandler.right)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.left)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
                case 4:
                    if (inputHandler.right || inputHandler.down)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.left || inputHandler.up)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
                case 5:
                    if (inputHandler.right || inputHandler.up)
                    {
                        if (!down)
                        {
                            currentButtonSelect++;
                            down = true;
                        }
                    }
                    else
                    {
                        down = false;
                    }

                    if (inputHandler.left || inputHandler.down)
                    {
                        if (!up)
                        {
                            currentButtonSelect--;
                            up = true;
                        }
                    }
                    else
                    {
                        up = false;
                    }
                    break;
            }

            if (currentButtonSelect >= buttons.Count)
            {
                currentButtonSelect = 0;
            }
            else if (currentButtonSelect < 0)
            {
                currentButtonSelect = buttons.Count - 1;
            }

            if (back)
            {
                titlePanel.SetActive(true);
                SceneManager.LoadScene(ListenToLoadMenu.sceneToReturnTo);
            }

            if (select && Metadata[currentButtonSelect].State == MetadataState.Success)
            {
                LoadLevel();
            }

            if (update && Metadata[currentButtonSelect].UpdateURL != "N/A")
            {
                System.Diagnostics.Process.Start(Metadata[currentButtonSelect].UpdateURL);
            }

            if (refresh) ReloadLevels();
        }

        private void SetText(GameObject toSelect)
        {
            selectedObject = toSelect;

            MetadataLevel meta = Metadata[currentButtonSelect];

            levelText[0].text = meta.Title;
            levelText[1].text = "Version: " + meta.Version;
            levelText[2].text = "Created By: " + meta.Author;

            if (GameDataManager.levelStats.ContainsKey(string.Concat(meta.Author, meta.Title, meta.Version).Replace(" ", "")))
            {
                string bTime = Game.GetTimerFormatted(Singleton<Game>.Instance.GetGameData().GetLevelStats(string.Concat(meta.Author, meta.Title, meta.Version).Replace(" ", "")).GetTimeBestMicroseconds());

                if (bTime != "16666:39.99")
                {
                    levelText[7].text = bTime;
                    SetMedal(StringToSeconds(bTime));
                } else
                {
                    levelText[7].text = "Not Completed";
                    SetMedal(999999);
                }
            } else
            {
                levelText[7].text = "Not Completed";
                SetMedal(999999);
            }

            string ftt = Utils.FloatToTime(float.Parse(meta.DevTime), "0:00.00");

            levelText[3].text = ftt;

            ftt = Utils.FloatToTime(float.Parse(meta.AceTime), "0:00.00");

            levelText[4].text = ftt;

            ftt = Utils.FloatToTime(float.Parse(meta.GoldTime), "0:00.00");

            levelText[5].text = ftt;

            ftt = Utils.FloatToTime(float.Parse(meta.SilverTime), "0:00.00");

            levelText[6].text = ftt;
        }

        private void SetMedal(float time)
        {
            MetadataLevel meta = Metadata[currentButtonSelect];

            if (time < float.Parse(meta.DevTime)) medal.sprite = medals[0];
            else if (time < float.Parse(meta.AceTime)) medal.sprite = medals[1];
            else if (time < float.Parse(meta.GoldTime)) medal.sprite = medals[2];
            else if (time < float.Parse(meta.SilverTime)) medal.sprite = medals[3];
            else if (Singleton<Game>.Instance.GetGameData().GetLevelStats(string.Concat(meta.Author, meta.Title, meta.Version).Replace(" ", "")).GetCompleted()) medal.sprite = medals[4];
            else medal.sprite = medals[5];
        }

        private float StringToSeconds(string str)
        {
            float result = 0;

            string[] stringData = str.Split(':');

            float minute = float.Parse(stringData[0]);

            float seconds = float.Parse(stringData[1]);

            minute *= 60;

            result = minute + seconds;

            return result;
        }

        private void SetMenuItemsPosition()
        {
            float currentPos = 0;
            float dirMod = 1;

            switch (buttonDir)
            {
                case 1:
                    currentPos = selectedObject.transform.position.x;
                    dirMod = -1;
                    break;
                case 4:
                    currentPos = selectedObject.transform.position.x;
                    dirMod = -1;
                    break;
                case 5:
                    currentPos = selectedObject.transform.position.x;
                    dirMod = -1;
                    break;
                default:
                    currentPos = selectedObject.transform.position.y;
                    break;
            }

            if (currentPos < buttonValley * scaleFactor)
            {
                Vector3 pos = buttonHolder.transform.position;
                pos.y += (buttonYDif * scaleFactor * dirMod);
                pos.x -= (buttonXDif * scaleFactor * dirMod);
                buttonHolder.transform.position = pos;
            }
            else if (currentPos > buttonPeak * scaleFactor)
            {
                Vector3 pos = buttonHolder.transform.position;
                pos.y -= (buttonYDif * scaleFactor * dirMod);
                pos.x += (buttonXDif * scaleFactor * dirMod);
                buttonHolder.transform.position = pos;
            }
            
        }

        private void LoadLevels()
        {
            List<Text> texts = new List<Text>
            {
                GameObject.Find("LevelName").GetComponent<Text>(),
                GameObject.Find("Version").GetComponent<Text>(),
                GameObject.Find("Author").GetComponent<Text>(),
                GameObject.Find("DevTime").GetComponent<Text>(),
                GameObject.Find("AceTime").GetComponent<Text>(),
                GameObject.Find("GoldTime").GetComponent<Text>(),
                GameObject.Find("SilverTime").GetComponent<Text>(),
                GameObject.Find("BestTime").GetComponent<Text>()
            };

            levelText = texts.ToArray();

            medal = GameObject.Find("uiMedal").GetComponent<Image>();

            List<Sprite> sprites = new List<Sprite>
            {
                GameObject.Find("devMedal").GetComponent<Image>().sprite,
                GameObject.Find("aceMedal").GetComponent<Image>().sprite,
                GameObject.Find("goldMedal").GetComponent<Image>().sprite,
                GameObject.Find("silverMedal").GetComponent<Image>().sprite,
                GameObject.Find("bronzeMedal").GetComponent<Image>().sprite,
                GameObject.Find("noMedal").GetComponent<Image>().sprite
            };

            medals = sprites.ToArray();

            Metadata.Clear();

            for (int i = 0; i < levelText.Length; i++)
            {
                levelText[i].text = string.Empty;
                levelText[i].color = Color.grey;
            }
            string[] files = Directory.GetFiles(levelDirectory, "level.json", SearchOption.AllDirectories);
            bool flag = files.Length == 0;
            if (flag)
            {
                levelText[0].text = "No Levels Available";
                levelsLoaded = false;
            }
            else
            {
                int j = 0;
                while (j < files.Length)
                {
                    string text = files[j];
                    try
                    {
                        bool flag2 = text.Remove(0, levelDirectory.Length + 1) == "level.json";
                        if (!flag2)
                        {
                            Metadata.Add(MetadataLevel.Load(text));

                            for (int k = 0; k < ModUpdater.modsWithUpdates[1].Count; k++)
                            {
                                if (Metadata[j].Title == ModUpdater.modsWithUpdates[1][k])
                                {
                                    Metadata[j].UpdateURL = ModUpdater.GetNewUpdateURL("Mod", Metadata[j].GamebananaID);
                                    Metadata[j].Title = string.Concat(Metadata[j].Title, " (Update)");
                                }
                            }
                        }
                    } catch
                    {

                    }
                    j++;
                    continue;
                }

                for (int i = 0; i < levelText.Length; i++)
                {
                    levelText[i].text = string.Empty;
                    levelText[i].color = Color.white;
                }

                levelsLoaded = true;
            }
        }

        private void CreateButtons()
        {
            for (int i = 0; i < Metadata.Count; i++)
            {
                GameObject button = Instantiate(buttonPrefab);
                button.transform.SetParent(buttonHolder.transform);
                button.transform.SetSiblingIndex(i);
                button.transform.localPosition = new Vector3(currentX, currentY, 0);
                if(i != 0)
                {
                    button.transform.Find("Highlight").gameObject.SetActive(false);
                }
                button.transform.Find("Text (Legacy)").GetComponent<Text>().text = Metadata[i].Title;

                button.AddComponent<LevelToggleButton>();

                buttons.Add(button);

                currentY -= buttonYDif;
                currentX += buttonXDif;
            }
        }

        private void SetVariables()
        {
            try
            {
                string[] dataMulti = DataReader.ReadDataMulti("ModListStartPosition");
                string data = DataReader.ReadData("ModListYDistance");

                buttonStart.x = float.Parse(dataMulti[0]);
                buttonStart.y = float.Parse(dataMulti[1]);
                buttonYDif = float.Parse(data);

                data = DataReader.ReadData("ModListXDistance");

                buttonXDif = float.Parse(data);

                data = DataReader.ReadData("ModListSelectionPeak");
                buttonPeak = float.Parse(data);

                data = DataReader.ReadData("ModListSelectionValley");
                buttonValley = float.Parse(data);

                data = DataReader.ReadData("ModListDirection");
                switch (data)
                {
                    case "Vertical":
                        buttonDir = 0;
                        break;
                    case "Horizontal":
                        buttonDir = 1;
                        break;
                    default:
                        float.Parse("Force Error");
                        break;
                }

                data = DataReader.ReadData("ModListAltInput");
                switch (data)
                {
                    case "Right":
                        if (buttonDir == 0)
                            buttonDir = 2;
                        else if (buttonDir == 1)
                            buttonDir = 4;
                        break;
                    case "Left":
                        if (buttonDir == 0)
                            buttonDir = 3;
                        else if (buttonDir == 1)
                            buttonDir = 5;
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                DataError();
            }
        }

        private void DataError()
        {
            error = true;

            string data = "";

            try
            {
                data = DataReader.ReadData("ModMenuScreenResolutionReference");
                float.Parse(data);
            }
            catch
            {
                data = "2160";
            }
            modmenu = GameObject.Find("ModMenu").GetComponent<Canvas>();
            float screenHeight = Screen.height;
            scaleFactor = screenHeight / float.Parse(data);
            modmenu.scaleFactor = scaleFactor;

            List<Text> texts = new List<Text>
            {
                GameObject.Find("ModName").GetComponent<Text>(),
                GameObject.Find("Author").GetComponent<Text>(),
                GameObject.Find("Description").GetComponent<Text>(),
                GameObject.Find("Version").GetComponent<Text>()
            };

            levelText = texts.ToArray();

            levelText[0].text = "";
            levelText[1].text = "";
            levelText[2].text = "Error Reading Data File." +
                "\n\nIf you made this version of RMMBY, please check the RMMBY wiki to make sure your data file is filled out correctly." +
                "\n\nIf you downloaded RMMBY, please reinstall RMMBY. If it still doesn't work, let the creator of this version of RMMBY know that there's an error in the data file.";
            levelText[3].text = "";
        }

        private void ReloadLevels()
        {
            foreach (GameObject button in buttons)
            {
                GameObject.Destroy(button);
            }

            buttons = new List<GameObject>();
            currentButtonSelect = 0;

            ModUpdater.CheckForUpdates("Levels", 1);

            LoadLevels();

            currentY = buttonStart.y;
            currentX = buttonStart.x;

            modmenu.scaleFactor = 1;
            CreateButtons();
            modmenu.scaleFactor = scaleFactor;
        }
    }
}
