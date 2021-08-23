using Bounce.TaleSpire.AssetManagement;
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
                Debug.Log($"Found tile: {tileId}");
                var state = new PlaceableLoadState
                {

                };
                // ____placeableInfoMap.Add(tileId,);
                Debug.Log(____placeableInfoMap[tileId].IsRegistered);
                Debug.Log(____placeableInfoMap[tileId].Index.Count);
            }
            return true;
        }
    }
}
