using System.Collections.Generic;
using Bounce.ManagedCollections;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Patches.UI_AssetBrowser;
using HarmonyLib;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "GetAllGroups")]
    internal class AssetDbGetAllGroupsPatch
    {
        public static void Postfix(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] __result)
        {
            var list = __result.ToList();
            list.Add(((AssetDb.DbEntry.EntryKind) Category.AuraAndEffects,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[0]));
            list.Add(((AssetDb.DbEntry.EntryKind) Category.Slab,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[1]));
            list.Add(((AssetDb.DbEntry.EntryKind) Category.Audio,
                UI_AssetBrowserSetupAssetIndexPatch._injecting[2]));
            __result = list.ToArray();
        }
    }

    [HarmonyPatch(typeof(AssetDb), "GetGroupsByKind")]
    internal class AssetDbGetGroupsByKindPatch
    {
        public static bool Prefix(
            ref AssetDb.DbEntry.EntryKind kind, 
            ref List<AssetDb.DbGroup> __result,
            ref List<AssetDb.DbGroup> ____tileGroups,
            ref List<AssetDb.DbGroup> ____creatureGroups,
            ref List<AssetDb.DbGroup> ____propGroups
        )
        {
            var actualKind = (Category) kind;
            switch (actualKind)
            {
                case Category.Tile:
                    __result = ____tileGroups;
                    break;
                case Category.Creature:
                    __result = ____creatureGroups;
                    break;
                case Category.Prop:
                    __result = ____propGroups;
                    break;
                case Category.AuraAndEffects:
                    __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[0];
                    break;
                case Category.Slab:
                    __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[1];
                    break;
                case Category.Audio:
                    __result = UI_AssetBrowserSetupAssetIndexPatch._injecting[2];
                    break;
                default:
                    __result = new List<AssetDb.DbGroup>();
                    break;
            }
            return false;
        }
    }
}