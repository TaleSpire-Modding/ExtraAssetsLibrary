using Bounce.Unmanaged;
using HarmonyLib;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(CreatureManager), "AddOrRequestAddCreature")]
    internal class CreatureManagerPatchAddOrRequestAddCreature
    {
        public static NGuid LastLoaded;

        private static void Postfix(ref CreatureDataV2 data,
            PlayerGuid[] owners,
            bool sync,
            bool spawnedByLoad)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(LastLoaded))
            {
                var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[LastLoaded];
                asset.PostCallback?.Invoke(LastLoaded, data.CreatureId);
                LastLoaded = NGuid.Empty;
            }
        }
    }
}