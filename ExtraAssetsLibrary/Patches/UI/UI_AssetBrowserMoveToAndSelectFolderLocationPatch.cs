using HarmonyLib;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "MoveToAndSelectFolderLocation")]
    internal class UI_AssetBrowserMoveToAndSelectFolderLocationPatch
    {
        public static bool Prefix(ref int folderIndex, ref bool select)
        {
            return true;
        }
    }
}