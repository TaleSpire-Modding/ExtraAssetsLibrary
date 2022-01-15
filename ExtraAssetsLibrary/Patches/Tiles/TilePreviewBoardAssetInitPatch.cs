using System.Collections.Generic;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(TilePreviewBoardAsset), "Init")]
    public class TilePreviewBoardAssetInitPatch
    {
        internal static List<NGuid> newDb = new List<NGuid>();
        public static NGuid LastLoaded;

        private static bool Prefix(ref TilePreviewBoardAsset __instance, NGuid boardAssetId, Vector3 position,
            Quaternion rotation,
            ref NGuid ___AssetId, ref int ___OrientationOffset, ref Bounds ___ColliderBoundsBound,
            ref AssetLoader[] ____assetLoaders,
            ref int ____assetsStillToLoad, ref GameObject ____container)
        {
            if (newDb.Contains(boardAssetId))
            {
                if (!AssetDb.Placeables.ContainsKey(boardAssetId))
                {
                    AssetDb.AddIdAsDummyPlaceable(boardAssetId, PlaceableKind.Tile);
                    if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:Create dummy for {boardAssetId}");
                }

                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log("Extra Asset Library Plugin:Load Exist");
                LastLoaded = boardAssetId;
                ___AssetId = boardAssetId;

                ___OrientationOffset = 0;
                ___ColliderBoundsBound = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), Vector3.one);
                var length = 1;
                ____assetLoaders = new AssetLoader[length];
                for (var index = 0; index < length; ++index)
                {
                    var gameObject = new GameObject("AssetLoader");
                    ____assetLoaders[index] = gameObject.AddComponent<AssetLoader>();
                }

                ____assetsStillToLoad = length;
                for (var index = 0; index < length; ++index)
                    ____assetLoaders[index].Init(__instance, ____container.transform,
                        new BlobView<AssetLoaderData.Packed>());
                __instance.transform.SetPositionAndRotation(position, rotation);
                return false;
            }

            LastLoaded = NGuid.Empty;
            return true;
        }
    }
}