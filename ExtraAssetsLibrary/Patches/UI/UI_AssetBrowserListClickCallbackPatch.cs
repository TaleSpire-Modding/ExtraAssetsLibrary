using HarmonyLib;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "ListClickCallback")]
    internal class UI_AssetBrowserListClickCallbackPatch
    {
        public static bool Prefix(UIListItemClickEvents obj)
        {
            return true;
        }
    }
}