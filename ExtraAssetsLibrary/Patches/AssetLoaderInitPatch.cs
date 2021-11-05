using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.Handlers;
using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetLoader), "Init")]
    public class AssetLoaderInitPatch
    {
        private static DictionaryList<NGuid, BlobView<AssetLoaderData.Packed>> blobs =
            new DictionaryList<NGuid, BlobView<AssetLoaderData.Packed>>();

        internal static bool stopSpawn;

        static bool Prefix(ref AssetLoader __instance, ref Transform ____transform, ref IAssetContainer ____assetContainer, IAssetContainer boardAsset, Transform parent,
            ref BlobView<AssetLoaderData.Packed> data)
        {
            if (AssetDbTryGetCreatureDataPatch.LastLoaded != NGuid.Empty)
            {
                Debug.Log("Extra Asset Library Plugin:Load Creature");
                GameObject model;

                if (AssetDbTryGetCreatureDataPatch.LoadedBefore != AssetDbTryGetCreatureDataPatch.LastLoaded)
                {
                    stopSpawn = false;
                    Debug.Log("Extra Asset Library Plugin:Loading Base");
                    model = UI_AssetBrowserSetupAssetIndexPatch.Bases.ContainsKey(AssetDbTryGetCreatureDataPatch.LastLoaded) ?
                        UI_AssetBrowserSetupAssetIndexPatch.Bases[AssetDbTryGetCreatureDataPatch.LastLoaded](AssetDbTryGetCreatureDataPatch.LastLoaded)
                        : BaseHelper.DefaultBase();
                    if (model == null)
                    {
                        stopSpawn = true;
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Extra Asset Library Plugin:Loading Root");
                    if (blobs.ContainsKey(AssetDbTryGetCreatureDataPatch.LastLoaded))
                    {
                        data = blobs[AssetDbTryGetCreatureDataPatch.LastLoaded];
                    }
                    else
                    {
                        model = UI_AssetBrowserSetupAssetIndexPatch.NguidMethods[AssetDbTryGetCreatureDataPatch.LastLoaded](AssetDbTryGetCreatureDataPatch.LastLoaded);
                        if (model == null)
                        {
                            model = new GameObject();
                            stopSpawn = true;
                            return false;
                        }
                        var blob = AssetLoadManagerInjectGameObjectAsAssetPatch.InjectGameObjectAsAssetPatch(
                            model,new float3(0,0,0),new quaternion(0,0,0,0),new float3(1,1,1),
                            AssetDbTryGetCreatureDataPatch.LastLoaded, AssetDbTryGetCreatureDataPatch.LastLoaded
                        );
                        blobs.Add(AssetDbTryGetCreatureDataPatch.LastLoaded, blob);
                        data = blob;
                    }
                    AssetDbTryGetCreatureDataPatch.LastLoaded = NGuid.Empty;
                    AssetDbTryGetCreatureDataPatch.LoadedBefore = NGuid.Empty;
                    return true;
                }
                ____assetContainer = boardAsset;
                ____transform = __instance.transform;
                ____transform.SetParent(parent, false);
                ____transform.localPosition = (Vector3)new float3(0, 0, 0);
                ____transform.localRotation = (Quaternion)new quaternion(0, 0, 0, 0);
                ____transform.localScale = (Vector3)new float3(1, 1, 1);

                string str = AssetDbTryGetCreatureDataPatch.LastLoaded.ToString();

                AssetDbTryGetCreatureDataPatch.LoadedBefore = AssetDbTryGetCreatureDataPatch.LastLoaded;
                __instance.OnAssetLoaded(AssetDbTryGetCreatureDataPatch.LastLoaded, str, model);

                Debug.Log("Extra Asset Library Plugin:Load Creature Done");
                return false;
            }
            if (TilePreviewBoardAssetInitPatch.LastLoaded != NGuid.Empty)
            {
                Debug.Log("Extra Asset Library Plugin:Loading Tile");
                if (blobs.ContainsKey(TilePreviewBoardAssetInitPatch.LastLoaded)) data = blobs[TilePreviewBoardAssetInitPatch.LastLoaded];
                else
                {
                    var model = UI_AssetBrowserSetupAssetIndexPatch.NguidMethods[TilePreviewBoardAssetInitPatch.LastLoaded](TilePreviewBoardAssetInitPatch.LastLoaded);
                    var blob = AssetLoadManagerInjectGameObjectAsAssetPatch.InjectGameObjectAsAssetPatch(
                        model, new float3(0, 0, 0), new quaternion(0, 0, 0, 0), new float3(1, 1, 1),
                        TilePreviewBoardAssetInitPatch.LastLoaded, TilePreviewBoardAssetInitPatch.LastLoaded
                    );
                    blobs.Add(TilePreviewBoardAssetInitPatch.LastLoaded, blob);
                    data = blob;
                }
                TilePreviewBoardAssetInitPatch.LastLoaded = NGuid.Empty;
                Debug.Log("Extra Asset Library Plugin:Load Tile Done");
                return true;
            }
            else
            {
                Debug.Log($"Extra Asset Library Plugin:ID:{data.Value.AssetPackId}");
            }
            return true;
        }
    }
}
