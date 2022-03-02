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
    internal class PalceableManagerLoadPlaceablePrototypePatch
    {
        private const string assembly = "PlaceableManager+LoadingTilePrototype,Bouncyrock.TaleSpire.AssetManagement, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
        public static void Prefix(NGuid assetPackId, string assetId, ref GameObject asset)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(assetPackId))
            {
                var x = UI_AssetBrowserSetupAssetIndexPatch.assets[assetPackId];
                asset = x.ModelCallback(assetPackId);
            }
        }
    }
}