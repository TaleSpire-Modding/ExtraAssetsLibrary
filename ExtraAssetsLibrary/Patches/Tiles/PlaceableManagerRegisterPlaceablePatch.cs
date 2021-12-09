using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(PlaceableManager), "RegisterPlaceable")]
    internal class PlaceableManagerRegisterPlaceablePatch
    {
        public static bool Prefix(NGuid placeableId, ref NativeHashMap<NGuid, PlaceableLoadState> ____placeableInfoMap)
        {
            if (TilePreviewBoardAssetInitPatch.newDb.Contains(placeableId))
            {
                Debug.Log($"Extra Asset Library Plugin:Found tile: {placeableId}");
                ____placeableInfoMap[placeableId] = new PlaceableLoadState(0, 1);
                return true;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlaceableManager), "ProcessAssetForPlaceable")]
    internal class PlaceableManagerProcessAssetForPlaceablePatch
    {
        public static bool Prefix(NGuid assetPackId, string fullyQualifiedAssetId, ref GameObject asset)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(assetPackId))
                asset = UI_AssetBrowserSetupAssetIndexPatch.assets[assetPackId].ModelCallback(assetPackId);
            return true;
        }
    }
}