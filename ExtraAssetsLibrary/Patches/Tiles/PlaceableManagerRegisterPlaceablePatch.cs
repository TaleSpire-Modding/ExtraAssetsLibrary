using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bounce;
using Bounce.BlobAssets;
using Bounce.ManagedCollections;
using Bounce.Singletons;
using Bounce.TaleSpire.AssetManagement;
using Bounce.TaleSpire.Physics;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;




namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(PlaceableManager), "RegisterPlaceable")]
    class PlaceableManagerRegisterPlaceablePatch
    {
        public static bool Prefix(NGuid tileId, ref NativeHashMap<NGuid, PlaceableLoadState> ____placeableInfoMap)
        {
            if (TilePreviewBoardAssetInitPatch.newDb.Contains(tileId))
            {
                Debug.Log($"Extra Asset Library Plugin:Found tile: {tileId}");
                ____placeableInfoMap[tileId] = new PlaceableLoadState(0, 1);
                return true;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlaceableManager), "ProcessAssetForPlaceable")]
    class PlaceableManagerProcessAssetForPlaceablePatch
    {
        public static bool Prefix(NGuid assetPackId, string fullyQualifiedAssetId, ref GameObject asset)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(assetPackId))
            {
                asset = UI_AssetBrowserSetupAssetIndexPatch.assets[assetPackId].ModelCallback(assetPackId);
            }
            return true;
        }
    }
}
