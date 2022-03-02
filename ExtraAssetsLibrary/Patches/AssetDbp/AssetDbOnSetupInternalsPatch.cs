using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using HarmonyLib;
using Newtonsoft.Json;
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
        // instantiate completed
        internal static bool HasInstantiated;
        // internal static BlobPtr<AssetLoaderData.Packed> clothBase;

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
            ref NativeList<BlobAssetReference<PlaceableData>> ____dummyPlaceables 
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

            //clothBase = _creatures
            //    .First().Value.Value.BaseAsset;

            foreach (var asset in UI_AssetBrowserSetupAssetIndexPatch.assets)
            {
                switch (asset.Value.Category)
                {
                    case Category.Creature:
                        InjectCreature(asset.Value);
                        break;
                    case Category.Tile:
                        InjectTiles(asset.Value);
                        break;
                    case Category.Prop:
                        InjectProps(asset.Value);
                        break;
                    default:
                        InjectCreature(asset.Value);
                        break;
                }
            }
            HasInstantiated = true;
        }

        internal static void InjectCreature(Asset asset)
        {
            Debug.Log($"Injecting Creature: {asset.Name}");
            var builder = new BlobBuilder(Allocator.Persistent);
            ref var root = ref builder.ConstructRoot <BlobArray<CreatureData>>();
            var assetArray = builder.Allocate(ref root,1);
            BlobView<AssetLoaderData.Packed> packed =
                InjectGameObjectAsAsset(null, float3.zero, quaternion.identity, float3.zero, NGuid.Empty);
            var builder2 = new BlobBuilder(Allocator.Persistent);
            ref var modelAsset = ref builder2.ConstructRoot<BlobPtr<AssetLoaderData.Packed>>();
            modelAsset.Value = packed.Value;

            assetArray[0] = new CreatureData
            {
                // BaseAsset = clothBase,
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

            var result = builder.CreateBlobAssetReference<BlobArray<CreatureData>>(Allocator.Persistent);
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var creaturesView = result.Value.TakeSubView(0,result.Value.Length);
            var methodInfo = typeof(AssetDb).GetMethod("PopulateCreatureIndex", flags);
            methodInfo.Invoke(null, new object[] {creaturesView});
            
            var icons = (Dictionary<NGuid,Sprite>) typeof(AssetDb).GetField("_icons",flags).GetValue(null);
            icons.Add(asset.Id, asset.Icon);
            typeof(AssetDb).GetField("_icons", flags).SetValue(null,icons);
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
            using (var blobBuilder = new BlobBuilder(Allocator.TempJob))
            {
                var builder = blobBuilder;
                ref var local = ref builder.ConstructRoot<AssetLoaderData.Packed>();
                new AssetLoaderData
                {
                    path = "_injected_",
                    assetName = nguid.ToString(),
                    position = position,
                    rotation = rotation,
                    scale = scale
                }.Pack(builder, nguid, ref local);
                var blobAssetReference = blobBuilder.CreateBlobAssetReference<AssetLoaderData.Packed>(Allocator.Persistent);
                // AssetLoadManager._injectedBlobData.Add(in blobAssetReference);
                // AssetLoadManager.Instance._assets.Add(blobAssetReference.Value.GenFullyQualifiedId(), src);
                return blobAssetReference.TakeView();
            }
        }

    }

}