using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.Handlers;
using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    
    // [HarmonyPatch(typeof(AssetLoader), "Init")]
    public class AssetLoaderInitPatch
    {
        private static readonly DictionaryList<NGuid, BlobView<AssetLoaderData.Packed>> blobs =
            new DictionaryList<NGuid, BlobView<AssetLoaderData.Packed>>();

        internal static bool stopSpawn;

        private static void Postfix()
        {
            if (stopSpawn)
            {
                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log("Extra Asset Library Plugin:Closing Spawner");
                stopSpawn = false;
                SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
            }
            /*else
            {
                CreatureSpawnBoardAssetSpawnPatch.respawn = !CreatureSpawnBoardAssetSpawnPatch.respawn;
                if (CreatureSpawnBoardAssetSpawnPatch.respawn && CreatureSpawnBoardAssetSpawnPatch.tinfo != null)
                {
                    SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
                    CreatureSpawnerBoardTool.SwitchCreatureTool(CreatureSpawnBoardAssetSpawnPatch.tinfo.Value);
                    CreatureSpawnBoardAssetSpawnPatch.tinfo = null;
                }
            }*/
        }

        private static bool Prefix(ref AssetLoader __instance, ref Transform ____transform,
            ref IAssetContainer ____assetContainer, IAssetContainer boardAsset, Transform parent,
            ref BlobView<AssetLoaderData.Packed> data)
        {
            if (AssetDbTryGetCreatureDataPatch.LastLoaded != NGuid.Empty)
            {
                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Load Creature");
                GameObject model;
                var id = AssetDbTryGetCreatureDataPatch.LastLoaded;
                var asset = ExtraAssetPlugin.RegisteredAssets[id];


                if (AssetDbTryGetCreatureDataPatch.LoadedBefore == NGuid.Empty)
                {
                    stopSpawn = false;
                    if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Loading Base");
                    model = asset.BaseCallback != null ? asset.BaseCallback(id) : BaseHelper.DefaultBase();
                    if (model == null)
                    {
                        stopSpawn = true;
                        return false;
                    }
                }
                else
                {
                    if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Loading Root");
                    if (blobs.ContainsKey(AssetDbTryGetCreatureDataPatch.LastLoaded))
                    {
                        data = blobs[AssetDbTryGetCreatureDataPatch.LastLoaded];
                    }
                    else
                    {
                        model = asset.ModelCallback(id);
                        if (model == null)
                        {
                            model = new GameObject();
                            stopSpawn = true;
                            CreatureSpawnBoardAssetSpawnPatch.respawn = false;
                            AssetDbTryGetCreatureDataPatch.LastLoaded = NGuid.Empty;
                            AssetDbTryGetCreatureDataPatch.LoadedBefore = NGuid.Empty;
                            return false;
                        }

                        var blob = AssetLoadManagerInjectGameObjectAsAssetPatch.InjectGameObjectAsAssetPatch(
                            model, new float3(0, 0, 0), new quaternion(0, 0, 0, 0), new float3(1, 1, 1),
                            AssetDbTryGetCreatureDataPatch.LastLoaded, AssetDbTryGetCreatureDataPatch.LastLoaded
                        );
                        blobs.Add(AssetDbTryGetCreatureDataPatch.LastLoaded, blob);
                        data = blob;
                        model.transform.localScale = model.transform.localScale/ ExtraAssetPlugin.RegisteredAssets[AssetDbTryGetCreatureDataPatch.LastLoaded].DefaultScale;
                    }

                    AssetDbTryGetCreatureDataPatch.LastLoaded = NGuid.Empty;
                    AssetDbTryGetCreatureDataPatch.LoadedBefore = NGuid.Empty;
                    return true;
                }

                ____assetContainer = boardAsset;
                ____transform = __instance.transform;
                ____transform.SetParent(parent, false);
                ____transform.localPosition = new float3(0, 0, 0);
                ____transform.localRotation = new quaternion(0, 0, 0, 0);
                ____transform.localScale = new float3(1, 1, 1);

                var str = AssetDbTryGetCreatureDataPatch.LastLoaded.ToString();

                AssetDbTryGetCreatureDataPatch.LoadedBefore = AssetDbTryGetCreatureDataPatch.LastLoaded;
                __instance.OnAssetLoaded(AssetDbTryGetCreatureDataPatch.LastLoaded, str, model);

                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Load Creature Done");
                return false;
            }

            else if (TilePreviewBoardAssetInitPatch.LastLoaded != NGuid.Empty)
            {
                var id = TilePreviewBoardAssetInitPatch.LastLoaded;
                var asset = ExtraAssetPlugin.RegisteredAssets[id];

                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Loading Tile");
                if (blobs.ContainsKey(TilePreviewBoardAssetInitPatch.LastLoaded))
                {
                    data = blobs[TilePreviewBoardAssetInitPatch.LastLoaded];
                }
                else
                {
                    var model = asset.ModelCallback(id);
                    var blob = AssetLoadManagerInjectGameObjectAsAssetPatch.InjectGameObjectAsAssetPatch(
                        model, new float3(0, 0, 0), new quaternion(0, 0, 0, 0), new float3(1, 1, 1),
                        TilePreviewBoardAssetInitPatch.LastLoaded, TilePreviewBoardAssetInitPatch.LastLoaded
                    );
                    blobs.Add(TilePreviewBoardAssetInitPatch.LastLoaded, blob);
                    data = blob;
                }

                TilePreviewBoardAssetInitPatch.LastLoaded = NGuid.Empty;
                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Load Tile Done");
                return true;
            }

            if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log($"Extra Asset Library Plugin:ID:{data.Value.AssetPackId}");
            return true;
        }
    }
}