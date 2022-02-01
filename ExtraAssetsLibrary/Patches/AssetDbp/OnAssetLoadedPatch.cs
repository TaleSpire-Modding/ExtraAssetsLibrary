using System.Linq;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetLoader), "OnAssetLoaded")]
    public class OnAssetLoadedPatch
    {
        public static bool Prefix(NGuid assetPackId, string assetId, ref GameObject prototype)
        {
            if (prototype.TryGetComponent(typeof(MeshFilter), out var meshFilter))
            {
                var myMesh = ((MeshFilter)meshFilter).mesh;
                if (myMesh == null) myMesh = prototype.GetComponentsInChildren<MeshFilter>().FirstOrDefault()?.mesh;
                if (myMesh != null)
                {
                    var collider = prototype.AddComponent<MeshCollider>();
                    collider.sharedMesh = myMesh;
                }
            }
            return true;
        }
    }
}
