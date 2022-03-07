using System.Collections.Generic;
using System.Reflection;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Asset = ExtraAssetsLibrary.DTO.Asset;

// ReSharper disable once CheckNamespace
namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "OnInstanceSetup")]
    internal class AssetDbOnSetupInternalsPatch
    {
        internal static bool Instantiated = false;
        internal static BlobPtr<AssetLoaderData.Packed> clothBase;
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        // Other
        internal static NativeList<BlobAssetReference<AssetPackIndex>> _assetPacksIndicies;
        internal static NativeHashMap<NGuid, BlobView<MusicData>> _music;

        // Placeables
        internal static NativeHashMap<NGuid, BlobView<PlaceableData>> _placeables;
        internal static NativeList<BlobAssetReference<Unity.Physics.Collider>> _placeableColliders;
        internal static CollisionFilter _explorerTiles;
        internal static CollisionFilter _explorerProps;
        internal static NativeList<BlobAssetReference<PlaceableData>> _dummyPlaceables;

        // Tiles
        internal static List<AssetDb.DbGroup> _tileGroups;
        internal static Dictionary<string, List<AssetDb.DbEntry>> _tilesByTag;
        internal static HashList<string> _tileTags;

        // Props
        internal static List<AssetDb.DbGroup> _propGroups;
        internal static Dictionary<string, List<AssetDb.DbEntry>> _propsByTag;
        internal static HashList<string> _propTags;

        // Creatures
        internal static NativeHashMap<NGuid, BlobView<CreatureData>> _creatures;
        internal static List<AssetDb.DbGroup> _creatureGroups;
        internal static Dictionary<string, List<AssetDb.DbEntry>> _creaturesByTag;
        internal static HashList<string> _creatureTags;

        public static void Postfix(
            ref NativeList<BlobAssetReference<AssetPackIndex>> ____assetPacksIndicies,
            ref NativeHashMap<NGuid, BlobView<PlaceableData>> ____placeables,
            ref NativeList<BlobAssetReference<Unity.Physics.Collider>> ____placeableColliders,
            ref NativeHashMap<NGuid, BlobView<CreatureData>> ____creatures,
            ref NativeHashMap<NGuid, BlobView<MusicData>> ____music,
            ref CollisionFilter ____explorerTiles,
            ref CollisionFilter ____explorerProps,
            ref NativeList<BlobAssetReference<PlaceableData>> ____dummyPlaceables,
            ref List<AssetDb.DbGroup> ____creatureGroups
        )
        {
            Debug.Log("Postfix DB Initialization");

            _assetPacksIndicies = ____assetPacksIndicies;
            _placeables = ____placeables;
            _music = ____music;
            _placeableColliders = ____placeableColliders;
            _creatures = ____creatures;
            _explorerTiles = ____explorerTiles;
            _explorerProps = ____explorerProps;
            _dummyPlaceables = ____dummyPlaceables;
            _creatureGroups = ____creatureGroups;
        }

        internal static void InjectCreature(Asset asset)
        {
            if (!Instantiated)
            {
                bool first = true;
                foreach (var creature in _creatures)
                {
                    if (first)
                    {
                        clothBase = creature.Value.Value.BaseAsset;
                        first = false;
                    }
                }

                Instantiated = true;
            }
            if (ExtraAssetPlugin.LogLevel.Value == LogLevel.All) Debug.Log($"Injecting Creature: {asset.Name}");
            var builder = new BlobBuilder(Allocator.Persistent);
            ref var root = ref builder.ConstructRoot <BlobArray<CreatureData>>();
            var assetArray = builder.Allocate(ref root,1);
            BlobView<AssetLoaderData.Packed> packed =
                InjectGameObjectAsAsset(asset.ModelCallback(asset.Id), float3.zero, quaternion.identity, float3.zero, asset.Id);
            var builder2 = new BlobBuilder(Allocator.Persistent);
            ref var modelAsset = ref builder2.ConstructRoot<BlobPtr<AssetLoaderData.Packed>>();
            modelAsset.Value = packed.Value;

            assetArray[0] = new CreatureData
            {
                BaseAsset = clothBase,
                Id = asset.Id,
                BaseRadius = asset.DefaultScale,
                DefaultScale = asset.DefaultScale,
                HeadPos = asset.headPos,
                HitPos = asset.hitPos,
                SpellPos = asset.spellPos,
                TorchPos = asset.torchPos,
                ModelAsset = modelAsset
            };
            builder.Allocate(ref assetArray[0].Tags, asset.tags.Length);
            for (int i = 0; i < asset.tags.Length; i++)
            {
                builder.AllocateString(ref assetArray[0].Tags[i], asset.tags[i]);
            }
            builder.AllocateString(ref assetArray[0].Name,asset.Name + "from Patch");
            builder.AllocateString(ref assetArray[0].Description,asset.Description);
            builder.AllocateString(ref assetArray[0].Group,asset.GroupName);
            
            var icons = (Dictionary<NGuid, Sprite>)typeof(AssetDb).GetField("_icons", flags).GetValue(null);
            icons.Add(asset.Id, asset.Icon);
            typeof(AssetDb).GetField("_icons", flags).SetValue(null, icons);

            var result = builder.CreateBlobAssetReference<BlobArray<CreatureData>>(Allocator.Persistent);
            var creaturesView = result.Value.TakeSubView(0,result.Value.Length);
            var methodInfo = typeof(AssetDb).GetMethod("PopulateCreatureIndex", flags);
            methodInfo.Invoke(null, new object[] {creaturesView});
        }

        internal static void InjectTiles(Asset asset)
        {
        }

        internal static void InjectProps(Asset asset)
        {
        }

        private static BlobView<AssetLoaderData.Packed> InjectGameObjectAsAsset(
            GameObject src,
            float3 position,
            quaternion rotation,
            float3 scale,
            NGuid nguid
            )
        {
            var builder = new BlobBuilder(Allocator.Persistent);
            ref var local = ref builder.ConstructRoot<AssetLoaderData.Packed>();
            new AssetLoaderData
            {
                path = "_injected_",
                assetName = nguid.ToString(),
                position = position,
                rotation = rotation,
                scale = scale
            }.Pack(builder, nguid, ref local);
            var blobAssetReference = builder.CreateBlobAssetReference<AssetLoaderData.Packed>(Allocator.Persistent);
            var i = 0;
            Debug.Log(i++);
            var _injectedBlobData = (NativeList<BlobAssetReference<AssetLoaderData.Packed>>) typeof(AssetLoadManager).GetField("_injectedBlobData", flags).GetValue(null);
            Debug.Log(i++);
            _injectedBlobData.Add(in blobAssetReference);

            Debug.Log(i++);
            typeof(AssetLoadManager).GetField("_injectedBlobData", flags).SetValue(null, _injectedBlobData);
            Debug.Log(i++);
            var _assets = (Dictionary<string, GameObject>)typeof(AssetLoadManager).GetField("_assets", flags).GetValue(AssetLoadManager.Instance);
            Debug.Log(i++);
            _assets.Add(blobAssetReference.Value.GenFullyQualifiedId(), src);
            Debug.Log(i++);
            typeof(AssetLoadManager).GetField("_assets", flags).SetValue(
                AssetLoadManager.Instance, _assets);
            Debug.Log(i++);
            return blobAssetReference.TakeView();
            
        }

    }

}