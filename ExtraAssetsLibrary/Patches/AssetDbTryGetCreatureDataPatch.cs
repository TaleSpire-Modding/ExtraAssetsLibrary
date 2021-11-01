using System.Collections.Generic;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "TryGetCreatureData")]
    public class AssetDbTryGetCreatureDataPatch
    {
        public static NGuid LastLoaded;
        public static NGuid LoadedBefore;
        internal static Dictionary<NGuid, BlobView<CreatureData>>
            newDb = new Dictionary<NGuid, BlobView<CreatureData>>();

        internal static bool canLoad = true;

        static bool Postfix(bool original, NGuid id, ref BlobView<CreatureData> data)
        {
            if (newDb.ContainsKey(id))
            {
                original = true;
                data = newDb[id];
                LastLoaded = id;
                if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(LastLoaded))
                {
                    var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[LastLoaded];
                    canLoad = asset.PreCallback == null || asset.PreCallback(id);
                }
                CreatureManagerPatchAddOrRequestAddCreature.LastLoaded = id;
            }
            else
            {
                LastLoaded = NGuid.Empty;
                LoadedBefore = NGuid.Empty;
            }
            return original;
        }
    }

}
