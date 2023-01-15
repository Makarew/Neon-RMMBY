namespace RMMBY.NeonLevelLoader
{
    public class MetadataLevel : MetadataBase
    {
        public string AssetBundleName { get; set; }
        public string DevTime { get; set; }
        public string AceTime { get; set; }
        public string GoldTime { get; set; }
        public string SilverTime { get; set; }
        public int EnvironmentType { get; set; }
        public bool Boof { get; set; }


        public new static MetadataLevel Load (string path)
        {
            MetadataLevel metadataLevel = MetadataBase.Load<MetadataLevel>(path);

            return metadataLevel;
        }
    }
}
