using System.Collections.Generic;
using MelonLoader;
using System.IO;

namespace RMMBY.NeonLevelLoader
{
    internal class CreateLevel
    {
        public static LevelData CreateLevelData()
        {
            string[] environment = new string[2];
            environment[0] = "Location_Game_GlassPort";
            environment[1] = "Content/Levels/Skyworld/GlassPort_Environment_Base";
            LevelData levelData = (LevelData)LevelData.CreateInstance("LevelData");
            levelData.name = "Level_Custom";
            levelData.ambience = "Bed_Wind_Water_Light";
            levelData.conversationAfterLevel = "";
            levelData.conversationBeforeLevel = "";
            levelData.difficultyRating = 1;
            levelData.discardLockData = new DiscardLockData[0];
            levelData.environmentLocationData = (LocationData)LocationData.CreateInstance("LocationData");
            levelData.environmentLocationData.locationName = environment[0];
            levelData.environmentScene = new SceneField(null, environment[1]);
            levelData.environmentSceneAlt = new SceneField(null, "");
            levelData.hasCollectible = false;
            levelData.isBossFight = false;
            levelData.isFinalLevel = false;
            levelData.isSidequest = false;
            levelData.levelDatabaseDirectory = "";
            levelData.levelDisplayName = "Custom Level";
            levelData.levelID = "CustomLevel";
            levelData.levelIntegerID = 100;
            levelData.levelRushType = LevelRush.LevelRushType.None;
            levelData.levelScene = new SceneField(null, "CustomLevel");
            levelData.music = "MUSIC_GAMEPLAY_GLASSOCEAN";
            levelData.particlePrewarmCounts = new List<LevelData.ParticlePrewarmPair>();
            levelData.prefabPrewarmCounts = new List<LevelData.PrefabPrewarmPair>();
            levelData.preventsPlayerFromQuitting = false;
            levelData.stats = new LevelStats();
            levelData.timeAce = 60;
            levelData.timeAce_Switch = 60;
            levelData.timeDev = 60;
            levelData.timeDev_Switch = 60;
            levelData.timeGold = 60;
            levelData.timeGold_Switch = 60;
            levelData.timeSilver = 60;
            levelData.timeSilver_Switch = 60;
            levelData.type = LevelData.LevelType.Level;
            levelData.useBookOfLifeLevelGoal = false;

            return levelData;
        }

        public static CampaignData CreateCampaign(HubContentData hub, HubContentData missionhub)
        {
            CampaignData campaignData = (CampaignData)CampaignData.CreateInstance("CampaignData");

            campaignData.name = "Campaign_Custom";
            campaignData.campaignDisplayName = "Custom Levels";
            campaignData.campaignID = "CUSTOM";
            campaignData.campaignType = CampaignData.CampaignType.MainQuest;
            campaignData.hubContentRepeating = hub;
            campaignData.missionData = new List<MissionData> { CreateMission(missionhub) };

            return campaignData;
        }

        public static MissionData CreateMission(HubContentData hub)
        {
            MissionData missionData = (MissionData)MissionData.CreateInstance("MissionData");

            missionData.name = "Mission_Custom";
            missionData.hubContentData = hub;
            missionData.levels = new List<LevelData> { CreateLevelData() };
            missionData.medalsRequired = 0;
            missionData.missionDialogues = new List<MissionDialogue>();
            missionData.missionDisplayName = "CUSTOM_MISSION";
            missionData.missionID = "CUSTOM_MISSION";
            missionData.missionType = MissionData.MissionType.SideQuest;
            missionData.musicID = "";

            return missionData;
        }

        public static void AddLevelCustomCampaign()
        {
            GameData gd = Singleton<Game>.Instance.GetGameData();

            if (gd.campaigns.Count > 2) return;

            gd.campaigns.Add(CreateCampaign(gd.campaigns[0].hubContentRepeating, gd.campaigns[0].missionData[0].hubContentData));
        }

        public static void LoadNewLevelData(string path)
        {
            MetadataLevel meta = MetadataLevel.Load(Path.Combine(path, "level.json"));

            Melon<Plugin>.Instance.currentLevel = meta;

            string[] environment = GetEnvironment(meta.EnvironmentType);

            GameData gd = Singleton<Game>.Instance.GetGameData();

            if(!GameDataManager.levelStats.ContainsKey(Melon<Plugin>.Instance.LevelID()))
            {
                GameDataManager.levelStats.Add(Melon<Plugin>.Instance.LevelID(), new LevelStats());
            }

            LevelData customLevel = gd.campaigns[2].missionData[0].levels[0];

            customLevel.environmentLocationData.locationName = environment[0];
            customLevel.environmentScene = new SceneField(null, environment[1]);

            customLevel.levelDisplayName = meta.Title;
            customLevel.levelID = Melon<Plugin>.Instance.LevelID();
            customLevel.timeDev = float.Parse(meta.DevTime);
            customLevel.timeDev_Switch = float.Parse(meta.DevTime);
            customLevel.timeAce = float.Parse(meta.AceTime);
            customLevel.timeAce_Switch = float.Parse(meta.AceTime);
            customLevel.timeGold = float.Parse(meta.GoldTime);
            customLevel.timeGold_Switch = float.Parse(meta.GoldTime);
            customLevel.timeSilver = float.Parse(meta.SilverTime);
            customLevel.timeSilver_Switch = float.Parse(meta.SilverTime);
            customLevel.useBookOfLifeLevelGoal = meta.Boof;
        }

        public static string[] GetEnvironment(int type)
        {
            string[] environment = new string[2];

            switch (type)
            {
                case 0:
                    environment[0] = "Location_Game_Sheol";
                    environment[1] = "Content/Levels/Afterlife/Afterlife_Environment_Green";
                    break;
                case 1:
                    environment[0] = "Location_Game_Sheol";
                    environment[1] = "Content/Levels/Afterlife/Afterlife_Environment_Red";
                    break;
                case 2:
                    environment[0] = "Location_Game_Sheol";
                    environment[1] = "Content/Levels/Afterlife/Afterlife_Environment_Violet";
                    break;
                case 3:
                    environment[0] = "Location_Game_Sheol";
                    environment[1] = "Content/Levels/Afterlife/Afterlife_Environment_Yellow";
                    break;
                case 4:
                    environment[0] = "Location_Game_Apocalypse";
                    environment[1] = "Content/Levels/Skyworld/Apocalypse_Environment_Base";
                    break;
                case 5:
                    environment[0] = "Location_Game_GlassPort";
                    environment[1] = "Content/Levels/Skyworld/GlassPort_Environment_Base";
                    break;
                case 6:
                    environment[0] = "Location_Game_ThirdTemple_Exterior";
                    environment[1] = "Content/Levels/Skyworld/GodTemple_Environment_Base";
                    break;
                case 7:
                    environment[0] = "Location_Game_ThirdTemple_Exterior";
                    environment[1] = "Content/Art/TEST/GodTemple_InteriorLighting_Test";
                    break;
                case 20:
                    environment[0] = "Location_Game_GlassPort_Climax";
                    environment[1] = "Content/Levels/Skyworld/HandOfGod_Environment_Base_LOOKDEV";
                    break;
                case 8:
                    environment[0] = "Location_Game_HangingGardens";
                    environment[1] = "Content/Levels/Skyworld/HangingGarden_Environment_Base";
                    break;
                case 9:
                    environment[0] = "Location_Game_HangingGardens";
                    environment[1] = "Content/Levels/Skyworld/HangingGarden_Bleak_Environment_Base";
                    break;
                case 10:
                    environment[0] = "Location_Game_HeavensEdge";
                    environment[1] = "Content/Levels/Skyworld/HeavensEdge_Environment_Base";
                    break;
                case 11:
                    environment[0] = "Location_Game_HeavensEdge";
                    environment[1] = "Content/Levels/Skyworld/HeavensEdgeTwilight_Environment_Base";
                    break;
                case 12:
                    environment[0] = "Location_Game_GlassPort";
                    environment[1] = "Content/Levels/Skyworld/Heaven_Environment_Base";
                    break;
                case 13:
                    environment[0] = "Location_Game_GlassPort";
                    environment[1] = "Content/Levels/Skyworld/Holy_Environment_Base";
                    break;
                case 14:
                    environment[0] = "Location_Game_LowerHeaven";
                    environment[1] = "Content/Levels/Skyworld/LowerHeaven_Environment_Base";
                    break;
                case 15:
                    environment[0] = "Location_Game_LowerHeaven";
                    environment[1] = "Content/Levels/Skyworld/LowerHeavenCloudy_Environment_Base";
                    break;
                case 16:
                    environment[0] = "Location_Game_LowerHeaven";
                    environment[1] = "Content/Levels/Skyworld/LowerHeavenOvercast_Environment_Base";
                    break;
                case 17:
                    environment[0] = "Location_Game_OldCity";
                    environment[1] = "Content/Levels/Skyworld/OldCity_Environment_Base";
                    break;
                case 18:
                    environment[0] = "Location_Game_GlassPort";
                    environment[1] = "Content/Levels/Skyworld/Origin_Environment_Base";
                    break;
                case 19:
                    environment[0] = "Location_Game_GlassPort";
                    environment[1] = "Content/Levels/Skyworld/Origin_Environment_Underwater_Cutscene";
                    break;
            }

            return environment;
        }
    }
}
