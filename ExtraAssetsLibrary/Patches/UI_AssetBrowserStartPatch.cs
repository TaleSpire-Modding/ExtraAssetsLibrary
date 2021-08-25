using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "Start")]
    class UI_AssetBrowserStartPatch
    {
        public static void Prefix()
        {
            // Debug.Log("Closing Spawner");
        }
    }
}
