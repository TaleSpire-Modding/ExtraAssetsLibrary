using System.Collections.Generic;
using Bounce.ManagedCollections;
using ExtraAssetsLibrary.AssetDbExtension;
using ExtraAssetsLibrary.DTO;
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
            
            list.Add(((AssetDb.DbEntry.EntryKind)CustomEntryKind.Aura, ExtraDb.extraGroups[CustomEntryKind.Aura]));
            list.Add(((AssetDb.DbEntry.EntryKind)CustomEntryKind.Slab, ExtraDb.extraGroups[CustomEntryKind.Slab]));
            
            __result = list.ToArray();
            Debug.Log($"Extra Asset Library Plugin: Added {__result.Length} Auras");
        }
    }
}
