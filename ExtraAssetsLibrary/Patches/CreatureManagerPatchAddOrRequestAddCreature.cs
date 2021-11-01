using Bounce.Unmanaged;
using DataModel;
using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(CreatureManager), "AddOrRequestAddCreature")]
    class CreatureManagerPatchAddOrRequestAddCreature
    {
        public static NGuid LastLoaded;

        static void Postfix(ref CreatureDataV2 data,
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
