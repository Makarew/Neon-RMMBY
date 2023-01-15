using Newtonsoft.Json;
using Steamworks;
using System.Collections.Generic;

namespace RMMBY.GameBanana
{
    public class ModData
    {
        [JsonIgnore]
        public string DownloadURL { get; set; }

        [JsonIgnore]
        public string ItemType { get; set; }

        [JsonIgnore]
        public int ItemID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Owner().name")]
        public string OwnerName { get; set; }

        [JsonProperty("Updates().bSubmissionsHasUpdates()")]
        public bool? HasUpdates { get; set; }

        [JsonProperty("Category().name")]
        public string Category { get; set; }

        [JsonProperty("Game().name")]
        public string GameName { get; set; }

        [JsonProperty("Updates().aGetLatestUpdates()")]
        public ModUpdate[] Updates { get; set; }

        [JsonProperty("Files().aFiles()")]
        public Dictionary<string, ModFile> Files { get; set; } = new Dictionary<string, ModFile>();

        public ModData(string downloadURL, string itemType, int itemID)
        {
            this.DownloadURL = downloadURL;
            this.ItemType = itemType;
            this.ItemID = itemID;
        }
    }
}
