using System.Collections.Generic;
using Bounce.ManagedCollections;
using ExtraAssetsLibrary.AssetDbExtension;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "GetAllGroups")]
    class AssetDbGetAllGroupsPatch
    {
        public static void Postfix(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] __result)
        {
            var list = __result.ToList();
            list.Add(((AssetDb.DbEntry.EntryKind)3,AuraDb._auraGroups));
            __result = list.ToArray();
            // Debug.Log($"Group Length:{__result.Length}");
        }
    }
}
