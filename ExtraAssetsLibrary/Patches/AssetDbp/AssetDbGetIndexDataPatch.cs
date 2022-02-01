using System;
using System.Collections.Generic;
using Bounce.Unmanaged;
using HarmonyLib;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "GetIndexData")]
    public class AssetDbGetIndexDataPatch
    {
        private static bool Prefix(ref Dictionary<NGuid, AssetDb.DbEntry> ____indexData, NGuid id,
            ref AssetDb.DbEntry __result)
        {
            try
            {
                __result = ____indexData[id];
                return false;
            }
            catch (Exception e)
            {
                ____indexData[id] = UI_AssetBrowserSetupAssetIndexPatch.GetInjecting(id);
                __result = ____indexData[id];
            }

            return false;
        }
    }
}