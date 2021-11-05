using System.Collections.Generic;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "TryGetCreatureData")]
    public class AssetDbTryGetCreatureDataPatch
    {
        public static NGuid LastLoaded;
        public static NGuid LoadedBefore;
        internal static Dictionary<NGuid, BlobView<CreatureData>>
            newDb = new Dictionary<NGuid, BlobView<CreatureData>>();

        static bool Postfix(bool original, NGuid id, ref BlobView<CreatureData> data)
        {
            if (newDb.ContainsKey(id))
            {
                original = true;
                data = newDb[id];
                LastLoaded = id;
                CreatureManagerPatchAddOrRequestAddCreature.LastLoaded = id;
            }
            else
            {
                LastLoaded = NGuid.Empty;
                LoadedBefore = NGuid.Empty;
            }
            return original;
        }
    }

}
