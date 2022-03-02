using System.Collections.Generic;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    // [HarmonyPatch(typeof(AssetLoadManager), "InjectGameObjectAsAsset")]
    public class AssetLoadManagerInjectGameObjectAsAssetPatch
    {
        public static NGuid InjectAssetNGuid;
        public static NGuid InjectPackNGuid;
        public static string FullyQualifiedIdReplacer;

        public static BlobView<AssetLoaderData.Packed> InjectGameObjectAsAssetPatch(GameObject src,
            float3 position,
            quaternion rotation,
            float3 scale, NGuid assetId, NGuid packId, string fullyQualifiedIdReplacer = "")
        {
            InjectAssetNGuid = assetId;
            InjectPackNGuid = packId;
            FullyQualifiedIdReplacer = fullyQualifiedIdReplacer;
            var o = AssetLoadManager.Instance.InjectGameObjectAsAsset(src, position, rotation, scale);
            InjectAssetNGuid = NGuid.Empty;
            InjectPackNGuid = NGuid.Empty;
            FullyQualifiedIdReplacer = "";
            return o;
        }

        private static bool Prefix(ref BlobView<AssetLoaderData.Packed> __result, GameObject src, float3 position,
            quaternion rotation, float3 scale,
            ref AssetLoadManager __instance,
            ref NativeList<BlobAssetReference<AssetLoaderData.Packed>> ____injectedBlobData,
            ref Dictionary<string, GameObject> ____assets)
        {
            if (InjectAssetNGuid != NGuid.Empty)
            {
                using (var blobBuilder = new BlobBuilder(Allocator.TempJob))
                {
                    var builder = blobBuilder;
                    ref var local = ref builder.ConstructRoot<AssetLoaderData.Packed>();
                    var nguid = InjectAssetNGuid;
                    new AssetLoaderData
                    {
                        path = "_injected_",
                        assetName = nguid.ToString(),
                        position = position,
                        rotation = rotation,
                        scale = scale
                    }.Pack(builder, InjectPackNGuid, ref local);
                    var blobAssetReference =
                        blobBuilder.CreateBlobAssetReference<AssetLoaderData.Packed>(Allocator.Persistent);
                    ____injectedBlobData.Add(in blobAssetReference);

                    if (!____assets.ContainsKey(string.IsNullOrWhiteSpace(FullyQualifiedIdReplacer)
                        ? blobAssetReference.Value.GenFullyQualifiedId()
                        : FullyQualifiedIdReplacer)) ____assets.Add(
                        string.IsNullOrWhiteSpace(FullyQualifiedIdReplacer)
                            ? blobAssetReference.Value.GenFullyQualifiedId()
                            : FullyQualifiedIdReplacer, src);

                    __result = blobAssetReference.TakeView();
                    if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:{blobAssetReference.Value.GenFullyQualifiedId()}");
                }

                return false;
            }

            return true;
        }
    }
}