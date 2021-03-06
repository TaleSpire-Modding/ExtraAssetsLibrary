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

        public static Dictionary<NGuid, Func<NGuid, GameObject>> NguidMethods =
            new Dictionary<NGuid, Func<NGuid, GameObject>>();

        public static Dictionary<NGuid, Func<NGuid, GameObject>> Bases =
            new Dictionary<NGuid, Func<NGuid, GameObject>>();

        public static Dictionary<NGuid, Asset> assets = new Dictionary<NGuid, Asset>();

        public static AssetDb.DbEntry GetInjecting(NGuid id)
        {
            foreach (var group in _injecting)
            foreach (var g in @group)
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
                    new List<AssetDb.DbGroup>(), // 
                    new List<AssetDb.DbGroup>(), // 
                    new List<AssetDb.DbGroup>(), // Auras
                    new List<AssetDb.DbGroup>(), // Effects
                    new List<AssetDb.DbGroup>(), // Slabs
                    new List<AssetDb.DbGroup>() // Audio
                };
        }

        public static AssetDb.DbGroup AddGroup(AssetDb.DbEntry.EntryKind kind, string groupName)
        {
            var groups = _injecting[(int) kind];
            if (groups.All(g => g.Name != groupName)) groups.Add(new AssetDb.DbGroup(groupName));
            return groups.Single(g => g.Name == groupName);
        }

        public static void AddEntity(AssetDb.DbEntry.EntryKind kind, string groupName,
            (AssetDb.DbEntry entity, CreatureData creatures, Func<NGuid, GameObject> callback)[] t)
        {
            AddGroup(kind, groupName);
            var groups = _injecting[(int) kind];
            var group = groups.Single(g => g.Name.Equals(groupName));
            group.Entries.AddRange(t.ToList().Select(e => e.entity));

            foreach (var tup in t) NguidMethods.Add(tup.creatures.Id, tup.callback);

            foreach (var en in t.ToList().Select(e => e.entity).Where(e =>
                !AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(e.Id) &&
                e.Kind == AssetDb.DbEntry.EntryKind.Creature))
            {
                var cd = t.ToList().Select(e => e.creatures).Single(c => c.Id == en.Id);
                // var asset = AssetLoadManager.Instance.InjectGameObjectAsAsset(,new float3(0,0,0), new quaternion(0,0,0,0), new float3(1,1,1));
                AssetDbTryGetCreatureDataPatch.newDb.Add(en.Id, BlobHandler.ToView(cd));
            }

            foreach (var en in t.ToList().Select(e => e.entity).Where(e =>
                !AssetDbTryGetCreatureDataPatch.newDb.ContainsKey(e.Id) && e.Kind == AssetDb.DbEntry.EntryKind.Tile))
                TilePreviewBoardAssetInitPatch.newDb.Add(en.Id);
        }


        public static void AddEntity(AssetDb.DbEntry.EntryKind kind, string groupName, AssetDb.DbEntry entity,
            CreatureData creatures, Func<NGuid, GameObject> callback)
        {
            AddEntity(kind, groupName, new[] {(entity, creatures, callback)});
        }


        private static void Prefix(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            Inject(ref all);
            // Print(ref all);
        }

        private static void Inject(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            for (var categoryIndex = 0; categoryIndex < all.Length; ++categoryIndex)
            {
                var kind = all[categoryIndex];
                var injecting = _injecting[(int) kind.Item1];
                var injectingGroups = injecting;
                foreach (var group in injectingGroups)
                    if (kind.Item2.Any(g => g.Name == @group.Name))
                    {
                        var loc = kind.Item2.Single(g => g.Name == @group.Name);
                        foreach (var e in @group.Entries)
                            if (loc.Entries.All(le => le.Id != e.Id))
                                loc.Entries.Add(e);
                    }
                    else
                    {
                        kind.Item2.Add(@group);
                    }

                var groups = kind.Item2.OrderBy(i => i.Name).ToList();
                all[categoryIndex] = (kind.Item1, groups);
            }

            var al = all.ToList();
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Aura, _injecting[3]));
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Effects, _injecting[4]));
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Slab, _injecting[5]));
            al.Add(((AssetDb.DbEntry.EntryKind) CustomEntryKind.Audio, _injecting[6]));
            all = al.ToArray();
        }

        private static void Print(ref (AssetDb.DbEntry.EntryKind, List<AssetDb.DbGroup>)[] all)
        {
            for (var categoryIndex = 0; categoryIndex < all.Length; ++categoryIndex)
            {
                var kind = all[categoryIndex];
                Debug.Log($"Extra Asset Library Plugin:Kind:{kind.Item1}");
                foreach (var group in kind.Item2)
                {
                    Debug.Log($"Extra Asset Library Plugin:Group:{group.Name}");
                    foreach (var entry in group.Entries) Debug.Log($"Extra Asset Library Plugin:Entry:{entry.Name}");
                }
            }
        }
    }
}