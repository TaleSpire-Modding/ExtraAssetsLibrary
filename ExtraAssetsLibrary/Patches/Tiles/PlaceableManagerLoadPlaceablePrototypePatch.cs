using System.Collections.Generic;
using System.Linq;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.Tiles
{

    /*
    [HarmonyPatch(typeof(PlaceableManager), "LoadPlaceablePrototype")]
    internal class PlaceableManagerLoadPlaceablePrototypePatch
    {
        public static void Prefix(NGuid assetPackId, string assetId, ref GameObject asset)
        {
            
        }
    }*/

    [HarmonyPatch(assembly, "OnAssetLoaded")]
    
    internal class PlaceableManagerLoadPlaceablePrototypePatch
    {
        private const string assembly = "PlaceableManager+LoadingTilePrototype,Bouncyrock.TaleSpire.AssetManagement, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        public static void Prefix(NGuid assetPackId, string assetId, ref GameObject asset)
        {
            Debug.Log($"Loaded:(pack:{assetPackId},id:{assetId})");
            if (ExtraAssetPlugin.RegisteredAssets.ContainsKey(assetPackId))
            {
                var x = ExtraAssetPlugin.RegisteredAssets[assetPackId];
                asset = x.ModelCallback(assetPackId);
                var tsResource = asset.AddComponent<TsAssetResources>();
                tsResource.SubAssetRenderInfo = asset.GetComponents<MeshRenderer>().Select(meshRenderer => new AssetMeshRenderer(meshRenderer)).ToArray();
            }
        }
    }
}