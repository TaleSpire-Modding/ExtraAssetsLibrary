using System;
using BepInEx;
using Bounce.TaleSpire.AssetManagement;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using ExtraAssetsLibrary.Patches;
using HarmonyLib;
using System.Reflection;
using Bounce.Unmanaged;
using UnityEngine;

namespace ExtraAssetsLibrary
{
    [BepInPlugin(Guid, Name, Version)]
    public class ExtraAssetPlugin:BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.TMC.plugins.ExtraAssetLib";
        public const string Version = "1.2.0.0";
        private const string Name = "TMCs' Extra Asset Library";

        public static void DoPatching()
        {
            var harmony = new Harmony(Guid);
            harmony.PatchAll();
        }

        void Awake()
        {
            Debug.Log($"{Name} is Active.");
            UI_AssetBrowserSetupAssetIndexPatch.initStatic();
            ModdingUtils.Initialize(this, Logger);
            DoPatching();
        }

        void Update()
        {
        }

        private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        /// <summary>
        /// This method should be run on awake by your plugin.
        /// </summary>
        /// <param name="audio">Description of your audio you are injecting.</param>
        private static void AddAudio(Audio audio)
        {

        }

        private static void AddCoreCallback(Func<NGuid> callback)
        {

        }

        /// <summary>
        /// This method should be run on awake by your plugin.
        /// </summary>
        /// <param name="asset">Description of your asset you are injecting.</param>
        public static void AddAsset(Asset asset)
        {
            
            Debug.Log($"Adding: {asset.Id}");
            if (!UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(asset.Id))
            {
                UI_AssetBrowserSetupAssetIndexPatch.assets.Add(asset.Id,asset);
            }
            var group = UI_AssetBrowserSetupAssetIndexPatch.AddGroup(asset.Kind, asset.GroupName);
            var tags = BlobHandler.ConstructBlobData(asset.tags);
            var entry = new AssetDb.DbEntry(asset.Id, asset.Kind, asset.Name,
                asset.Description, group, group.Name, asset.groupTagOrder, ref tags, asset.isDeprecated, asset.Icon);

            var cd = new CreatureData
            {
                Id = asset.Id,
                // ModelAsset = *modelAsset.Value,
                Tags = tags,
                DefaultScale = asset.DefaultScale,
                BaseRadius = 1,
            };

            if (asset.BaseCallback != null) UI_AssetBrowserSetupAssetIndexPatch.Bases.Add(asset.Id,asset.BaseCallback);
            UI_AssetBrowserSetupAssetIndexPatch.AddEntity(asset.Kind,entry.GroupTagName,entry,cd,asset.ModelCallback);
            Debug.Log($"{asset.Id} Added");
        }
    }
}
