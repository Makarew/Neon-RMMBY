using Newtonsoft.Json;
using System;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RMMBY.Helpers;
using System.IO;

namespace RMMBY.GameBanana
{
    public static class ModUpdater
    {
        public static List<string>[] modsWithUpdates = new List<string>[2];

        public static void CheckForUpdates(string folder, int listNum)
        {
            Melon<Plugin>.Logger.Msg("Checking For Updates");

            List<MetadataBase> Metadata = new List<MetadataBase>();

            if (modsWithUpdates[listNum] == null)
                modsWithUpdates[listNum] = new List<string>();
            else
                modsWithUpdates[listNum].Clear();

            string path = Path.Combine(DataReader.ReadData("datapath").Replace("\\data", ""), folder);
            string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            bool flag = files.Length == 0;
            if (flag)
            {
                Melon<Plugin>.Logger.Msg("No Mods Found");
            }
            else
            {
                int i = 0;
                while (i < files.Length)
                {
                    string text = files[i];
                    try
                    {
                        Metadata.Add(MetadataBase.Load<MetadataBase>(text));

                        if (Metadata[i].GamebananaID != -1)
                        {
                            if (ModUpdater.CheckForUpdate("Mod", Metadata[i].Version, Metadata[i].GamebananaID))
                            {
                                Melon<Plugin>.Logger.Msg(Metadata[i].Title + " has an update.");

                                modsWithUpdates[listNum].Add(Metadata[i].Title);
                            }
                        }
                    }
                    catch
                    {

                    }
                    i++;
                    continue;
                }
            }
        }

        public static bool CheckForUpdate(string itemType, string installedVersion, int itemID)
        {
            bool result = false;

            //Files().aFiles()

            ModData modData = GetModData(string.Format("gb.com/dl/123456,{0},{1}", itemType, itemID));



            string[] newestVersionSplit = modData.Updates[0].Version.Split('.');
            string[] installedVersionSplit = installedVersion.Split('.');

            if (float.Parse(newestVersionSplit[0]) > float.Parse(installedVersionSplit[0]))
            {
                result = true;
            } else if (float.Parse(newestVersionSplit[0]) == float.Parse(installedVersionSplit[0]) && 
                float.Parse(newestVersionSplit[1]) > float.Parse(installedVersionSplit[1]))
            {
                result = true;
            } else if (installedVersionSplit.Length > 2)
            {
                if (float.Parse(newestVersionSplit[0]) == float.Parse(installedVersionSplit[0]) &&
                float.Parse(newestVersionSplit[1]) == float.Parse(installedVersionSplit[1]) &&
                float.Parse(newestVersionSplit[2]) > float.Parse(installedVersionSplit[2]))
                {
                    result = true;
                }
            }

            return result;
        }

        public static string GetNewUpdateURL(string itemType, int itemID)
        {
            string result = "N/A";

            ModData modData = GetModData(string.Format("gb.com/dl/123456,{0},{1}", itemType, itemID));
            string archiveID = modData.Files.First().Key;

            result = string.Format("rmmby://https://gamebanana.com/mmdl/{2},{0},{1}", itemType, itemID, archiveID);

            return result;
        }

        public static ModData GetModData(string url)
        {
            string[] array = url.Split(new char[]
            {
                ','
            });

            if (array.Length < 3)
                return null;

            int num = "rmmby".Length + 3;

            string downloadUrl = array[0].Substring(num, array[0].Length - num);
            string itemType = array[1];
            int itemId;

            int.TryParse(array[2], out itemId);

            return GetItemData(downloadUrl, itemType, itemId);
        }

        public static ModData GetItemData(string downloadUrl, string itemType, int itemId)
        {
            ModData itemData = new ModData(downloadUrl, itemType, itemId);
            string requestUri = CreateRequestUrl<ModData>(itemData, itemType, itemId);

            ModData itemData2 = JsonConvert.DeserializeObject<ModData>(Uri.UnescapeDataString(Client.Get().GetStringAsync(requestUri).Result));
            if (itemData2 != null)
            {
                itemData = itemData2;
            }

            itemData.DownloadURL = downloadUrl;
            itemData.ItemType = itemType;
            itemData.ItemID = itemId;
            return itemData;
        }

        public static string CreateRequestUrl<T>(T item, string itemType, int itemID)
        {
            List<string> supportedFields = GetSupportedFields(itemType);
            string text = string.Format("https://api.gamebanana.com/Core/Item/Data?itemtype={0}&itemid={1}&fields=", itemType, itemID);
            PropertyInfo[] properties = item.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                JsonPropertyAttribute jsonPropertyAttribute = (JsonPropertyAttribute)properties[i].GetCustomAttribute(typeof(JsonPropertyAttribute));

                if (jsonPropertyAttribute != null && supportedFields.Contains(jsonPropertyAttribute.PropertyName))
                {
                    if (text.Last<char>() != '=')
                    {
                        text += ",";
                    }
                    text += jsonPropertyAttribute.PropertyName;
                }
            }

            return text + "&format=json&return_keys=1";
        }

        public static List<string> GetSupportedFields(string itemType)
        {
            List<string> result;
            try
            {
                result = JsonConvert.DeserializeObject<List<string>>(Client.Get().GetStringAsync("https://api.gamebanana.com/Core/Item/Data/AllowedFields?itemtype=" + itemType).Result);
            }
            catch
            {
                result = new List<string>();
            }
            return result;
        }
    }
}
