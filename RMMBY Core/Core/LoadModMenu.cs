using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using MelonLoader;

namespace RMMBY
{
    public class LoadModMenu
    {
        private static AssetBundle bundle;
        public static string scene;

        public static void CheckForBundle(string sceneName)
        {
            try
            {
                if (bundle == null)
                {
                    string path = Path.Combine(MelonHandler.ModsDirectory, "RMMBY/rmmby");
                    bundle = AssetBundle.LoadFromFile(path);
                }

                Melon<Plugin>.Logger.Msg("Found Bundle");
            }
            catch
            {
                Melon<Plugin>.Logger.Msg("Can't Find Bundle");
            }

            try
            {
                SceneManager.LoadScene(sceneName);
                Melon<Plugin>.Logger.Msg("Loading Scene");
            }
            catch
            {
                Melon<Plugin>.Logger.Msg("Can't Find Scene");
            }
        }
    }
}