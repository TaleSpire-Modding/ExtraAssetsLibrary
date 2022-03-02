using System.Collections.Generic;
using Bounce.ManagedCollections;
using ExtraAssetsLibrary.DTO;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    // [HarmonyPatch(typeof(AssetDb), "GetAllGroups")]
    internal class AssetDbGetAllGroupsPatch
    {
        public static void Postfix(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] __result)
        {
            var list = __result.ToList();
            list.Add(((AssetDb.DbEntry.EntryKind) Category.AuraAndEffects,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[3]));
            list.Add(((AssetDb.DbEntry.EntryKind) Category.Slab,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[4]));
            list.Add(((AssetDb.DbEntry.EntryKind) Category.Audio,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[5]));
            __result = list.ToArray();
            if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log($"Extra Asset Library Plugin: Added {__result.Length} Catagories");
        }
    }

    // [HarmonyPatch(typeof(AssetDb), "GetGroupsByKind")]
    internal class AssetDbGetGroupsByKindPatch
    {
        public static void Postfix(ref AssetDb.DbEntry.EntryKind kind, ref List<AssetDb.DbGroup> __result)
        {
            var actualKind = (Category) kind;
            if (actualKind == Category.AuraAndEffects) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
            if (actualKind == Category.Slab) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
            if (actualKind == Category.Audio) __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[5];
        }
    }
}