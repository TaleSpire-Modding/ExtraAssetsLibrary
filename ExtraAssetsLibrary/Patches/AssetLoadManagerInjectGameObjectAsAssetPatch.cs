using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetLibrary.Patches
{
    [HarmonyPatch(typeof(AssetLoadManager), "InjectGameObjectAsAsset")]
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

        static bool Prefix(ref BlobView<AssetLoaderData.Packed> __result, GameObject src, float3 position, quaternion rotation, float3 scale, 
            ref AssetLoadManager __instance, ref NativeList<BlobAssetReference<AssetLoaderData.Packed>>  ____injectedBlobData, 
            ref Dictionary<string, GameObject> ____assets)
        {
            if (InjectAssetNGuid != NGuid.Empty)
            {
                using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.TempJob))
                {
                    BlobBuilder builder = blobBuilder;
                    ref AssetLoaderData.Packed local = ref builder.ConstructRoot<AssetLoaderData.Packed>();
                    NGuid nguid = InjectAssetNGuid;
                    new AssetLoaderData
                    {
                        path = "_injected_",
                        assetName = nguid.ToString(),
                        position = ((Vector3) position),
                        rotation = ((Quaternion) rotation),
                        scale = ((Vector3) scale)
                    }.Pack(builder, InjectPackNGuid, ref local);
                    BlobAssetReference<AssetLoaderData.Packed> blobAssetReference =
                        blobBuilder.CreateBlobAssetReference<AssetLoaderData.Packed>(Allocator.Persistent);
                    ____injectedBlobData.Add(in blobAssetReference);

                    ____assets.Add(
                        string.IsNullOrWhiteSpace(FullyQualifiedIdReplacer)
                            ? blobAssetReference.Value.GenFullyQualifiedId()
                            : FullyQualifiedIdReplacer, src);

                    __result = blobAssetReference.TakeView<AssetLoaderData.Packed>();
                    Debug.Log(blobAssetReference.Value.GenFullyQualifiedId());
                }
                return false;
            }
            return true;
        }

    }
}
