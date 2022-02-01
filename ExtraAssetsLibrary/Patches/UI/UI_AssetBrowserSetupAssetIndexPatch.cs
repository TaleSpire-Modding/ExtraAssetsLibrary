using System;
using System.Collections.Generic;
using System.Linq;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "SetupAssetIndex")]
    public class UI_AssetBrowserSetupAssetIndexPatch
    {
        internal static List<AssetDb.DbGroup>[] _injecting;

        public static Dictionary<NGuid, Asset> assets = new Dictionary<NGuid, Asset>();

        public static AssetDb.DbEntry GetInjecting(NGuid id)
        {
            foreach (var group in _injecting)
            foreach (var g in group)
                if (g.Entries.Any(i => i.Id == id))
                    return g.Entries.Single(i => i.Id == id);
            return null;
        }

        public static void initStatic()
        {
            if (_injecting == null)
                _injecting = new[]
                {
                    new List<AssetDb.DbGroup>(), // Tiles
                    new List<AssetDb.DbGroup>(), // Minis
                    new List<AssetDb.DbGroup>(), // Props
                    new List<AssetDb.DbGroup>(), // Auras and Effects
                    new List<AssetDb.DbGroup>(), // Audio
                    new List<AssetDb.DbGroup>(), // Slabs
                };
        }

        public static AssetDb.DbGroup AddGroup(Category kind, string groupName)
        {
            var groups = _injecting[(int) kind];
            if (groups.All(g => g.Name != groupName)) groups.Add(new AssetDb.DbGroup(groupName));
            return groups.Single(g => g.Name == groupName);
        }

        public static void AddEntity(Category kind, string groupName,
            (AssetDb.DbEntry entity, CreatureData creatures, Func<NGuid, GameObject> callback)[] t)
        {
            AddGroup(kind, groupName);
            var groups = _injecting[(int) kind];
            var group = groups.Single(g => g.Name.Equals(groupName));
            group.Entries.AddRange(t.ToList().Select(e => e.entity));

            foreach (var en in t.ToList().Select(e => e.entity).Where(e =>
                !AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(e.Id) &&
                e.Kind == AssetDb.DbEntry.EntryKind.Creature))
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
            Inject(ref all);
        }

        private static void Inject(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            for (var categoryIndex = 0; categoryIndex < all.Length; ++categoryIndex)
            {
                var kind = all[categoryIndex];
                var injecting = _injecting[(int) kind.Item1];
                var injectingGroups = injecting;
                foreach (var group in injectingGroups)
                    if (kind.Item2.Any(g => g.Name == group.Name))
                    {
                        var loc = kind.Item2.Single(g => g.Name == group.Name);
                        foreach (var e in group.Entries)
                            if (loc.Entries.All(le => le.Id != e.Id))
                                loc.Entries.Add(e);
                    }
                    else
                    {
                        kind.Item2.Add(group);
                    }

                var groups = kind.Item2.OrderBy(i => i.Name).ToList();
                all[categoryIndex] = (kind.Item1, groups);
            }

            var al = all.ToList();
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Aura, _injecting[3]));
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Slab, _injecting[4]));
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Audio, _injecting[5]));
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