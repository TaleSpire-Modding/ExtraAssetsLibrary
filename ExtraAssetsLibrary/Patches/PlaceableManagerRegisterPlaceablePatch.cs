﻿using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using UnityEngine;

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
                var state = new PlaceableLoadState
                {

                };
                Debug.Log($"Extra Asset Library Plugin:{____placeableInfoMap[tileId].IsRegistered}");
                Debug.Log($"Extra Asset Library Plugin:{____placeableInfoMap[tileId].Index.Count}");
            }
            return true;
        }
    }
}
