using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI_AssetBrowser
{
    [HarmonyPatch(typeof(global::UI_AssetBrowser), "SetupAssetIndex")]
    public class UI_AssetBrowserSetupAssetIndexPatch
    {
        internal static List<AssetDb.DbGroup>[] _injecting = 
        {
            new List<AssetDb.DbGroup>
            {
                new AssetDb.DbGroup("Empty")
                {
                    
                }
            }, // Auras and Effects
            new List<AssetDb.DbGroup>{
                new AssetDb.DbGroup("Empty")
                {

                }
            }, // Audio
            new List<AssetDb.DbGroup>{
                new AssetDb.DbGroup("Empty")
                {
                    
                }
            }, // Slabs
        };

        public static AssetDb.DbEntry GetInjecting(NGuid id)
        {
            foreach (var group in _injecting)
            foreach (var g in group)
                if (g.Entries.Any(i => i.Id == id))
                    return g.Entries.Single(i => i.Id == id);
            return null;
        }

        public static AssetDb.DbGroup AddGroup(Category kind, string groupName)
        {
            var groups = _injecting[(int)kind - 3];
            if (groups.Count == 1 && groups[0].Name == "Empty") groups.RemoveAt(0);
            if (groups.All(g => g.Name != groupName)) groups.Add(new AssetDb.DbGroup(groupName));
            return groups.Single(g => g.Name == groupName);
        }

        public static void AddEntity(Category kind, string groupName,
            (AssetDb.DbEntry entity, CreatureData creatures, Func<NGuid, GameObject> callback)[] t)
        {
            AddGroup(kind, groupName);
            var groups = _injecting[(int) kind - 3];
            var group = groups.Single(g => g.Name.Equals(groupName));
            group.Entries.AddRange(t.ToList().Select(e => e.entity));

            foreach (var en in t.ToList().Select(e => e.entity).Where(e =>
                !AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(e.Id) &&(
                e.Kind == AssetDb.DbEntry.EntryKind.Creature || (int)e.Kind > 3)))
            {
                var cd = t.ToList().Select(e => e.creatures).Single(c => c.Id == en.Id);
                if (!AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(en.Id))
                {
                    
                    AssetDbTryGetCreatureDataPatch.newDb.Add(en.Id, BlobHandler.ToView(cd));
                }
            }

            foreach (var en in t.ToList().Select(e => e.entity).Where(e =>
                !AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(e.Id) && e.Kind == AssetDb.DbEntry.EntryKind.Tile))
                TilePreviewBoardAssetInitPatch.newDb.Add(en.Id);
        }


        public static void AddEntity(Category kind, string groupName, AssetDb.DbEntry entity,
            CreatureData creatures, Func<NGuid, GameObject> callback)
        {
            AddEntity(kind, groupName, new[] {(entity, creatures, callback)});
        }


        private static void Prefix(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            var al = all.ToList();
            al.Add(((AssetDb.DbEntry.EntryKind)Category.AuraAndEffects, _injecting[0]));
            al.Add(((AssetDb.DbEntry.EntryKind)Category.Slab, _injecting[1]));
            al.Add(((AssetDb.DbEntry.EntryKind)Category.Audio, _injecting[2]));
            all = al.ToArray();
        }

        private static void Print(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            for (var categoryIndex = 0; categoryIndex < all.Length; ++categoryIndex)
            {
                var kind = all[categoryIndex];
                if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:Kind:{kind.Item1}");
                foreach (var group in kind.Item2)
                {
                    if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:Group:{group.Name}");
                    foreach (var entry in group.Entries) if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log($"Extra Asset Library Plugin:Entry:{entry.Name}");
                }
            }
        }
    }
}