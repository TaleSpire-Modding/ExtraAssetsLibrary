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
            var aura = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
            var effects = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
            var actual = ExtraDb.Zip(aura, effects);
            list.Add(((AssetDb.DbEntry.EntryKind)CustomEntryKind.Aura, actual));
            list.Add(((AssetDb.DbEntry.EntryKind)CustomEntryKind.Slab, UI_AssetBrowserSetupAssetIndexPatch._injecting[5]));
            list.Add(((AssetDb.DbEntry.EntryKind)CustomEntryKind.Audio, UI_AssetBrowserSetupAssetIndexPatch._injecting[6]));

            __result = list.ToArray();
            Debug.Log($"Extra Asset Library Plugin: Added {__result.Length} Catagories");
        }
    }

    [HarmonyPatch(typeof(AssetDb), "GetGroupsByKind")]
    class AssetDbGetGroupsByKindPatch
    {
        public static void Postfix(ref AssetDb.DbEntry.EntryKind kind, ref List<AssetDb.DbGroup> __result)
        {
            var actualKind = (CustomEntryKind) kind;
            if (actualKind == CustomEntryKind.Aura) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
            if (actualKind == CustomEntryKind.Effects) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
            if (actualKind == CustomEntryKind.Slab) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[5];
            if (actualKind == CustomEntryKind.Audio) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[6];
        }
    }
}
