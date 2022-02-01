using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using ExtraAssetsLibrary.Patches;
using HarmonyLib;
using LordAshes;
using UnityEngine;

namespace ExtraAssetsLibrary
{
    public enum LogLevel
    {
        None,
        Low,
        Medium,
        High,
        All,
    }


    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(FileAccessPlugin.Guid)]
    public class ExtraAssetPlugin : BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.TMC.plugins.ExtraAssetLib";
        public const string Version = "1.3.0.0";
        private const string Name = "HolloFoxes' Extra Asset Library";

        internal static ConfigEntry<bool> AutoClear { get; set; }
        internal static ConfigEntry<LogLevel> LogLevel { get; set; }

        private const BindingFlags bindFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        internal static Dictionary<string, Action<Category>> OnCatagoryChange =
            new Dictionary<string, Action<Category>>();

        /// <summary>
        ///     List of all callbacks being run on an asset being loaded
        /// </summary>
        public static DictionaryList<string, Func<NGuid, AssetDb.DbEntry.EntryKind, bool>> CoreAssetPrefixCallbacks =
            new DictionaryList<string, Func<NGuid, AssetDb.DbEntry.EntryKind, bool>>();

        public static void DoPatching()
        {
            var harmony = new Harmony(Guid);
            harmony.PatchAll();
        }

        

        public static void DoConfig(ConfigFile Config)
        {
            AutoClear = Config.Bind("Mini Loading", "Auto Clear Failed Minis", false);
            LogLevel = Config.Bind("Logging", "Level", ExtraAssetsLibrary.LogLevel.Low);
        }

        private void Awake()
        {
            Debug.Log("AQN:"+typeof(PlaceableManager.IPlaceableLoadSubscriber).AssemblyQualifiedName);

            DoConfig(Config);
            if (LogLevel.Value > ExtraAssetsLibrary.LogLevel.None) Debug.Log($"Extra Asset Library Plugin:{Name} is Active.");
            UI_AssetBrowserSetupAssetIndexPatch.initStatic();
            DoPatching();
            
        }

        internal static bool ClothBaseLoaded;
        internal static bool Reloaded;

        
        public static void AddOnCatagoryChange(string Guid, Action<Category> Callback)
        {
            if (!OnCatagoryChange.ContainsKey(Guid)) OnCatagoryChange.Add(Guid, Callback);
        }

        public static void RemoveCatagoryChange(string Guid)
        {
            OnCatagoryChange.Remove(Guid);
        }

        /// <summary>
        ///     This method should be run on awake by your plugin.
        /// </summary>
        /// <param name="asset">Description of your asset you are injecting.</param>
        public static void AddAsset(Asset asset)
        {
            if (asset.CustomKind == CustomEntryKind.Projectile)
            {
                return;
            }

            if ((int)asset.Category >= 7)
            {
                switch (asset.CustomKind)
                {
                    case CustomEntryKind.Creature:
                        asset.Category = Category.Creature;
                        break;
                    case CustomEntryKind.Tile:
                        asset.Category = Category.Tile;
                        break;
                    case CustomEntryKind.Projectile:
                        asset.Category = Category.AuraAndEffects;
                        break;
                    case CustomEntryKind.Aura:
                        asset.Category = Category.AuraAndEffects;
                        break;
                    case CustomEntryKind.Effects:
                        asset.Category = Category.AuraAndEffects;
                        break;
                    case CustomEntryKind. Prop:
                        asset.Category = Category.Prop;
                        break;
                    case CustomEntryKind.Audio:
                        asset.Category = Category.Audio;
                        break;
                    case CustomEntryKind.Slab:
                        asset.Category = Category.Slab;
                        break;
                    default:
                        asset.Category = Category.Creature;
                        break;
                }
            }

            if (LogLevel.Value >= ExtraAssetsLibrary.LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:Adding: {asset.Id}");
            if (!UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(asset.Id))
                UI_AssetBrowserSetupAssetIndexPatch.assets.Add(asset.Id, asset);
            var group = UI_AssetBrowserSetupAssetIndexPatch.AddGroup(asset.Category, asset.GroupName);
            var tags = BlobHandler.ConstructBlobData(asset.tags);
            var entry = new AssetDb.DbEntry(asset.Id, asset.Kind, asset.Name,
                asset.Description, group, group.Name, asset.groupTagOrder, ref tags, asset.isDeprecated, asset.Icon);

            var cd = new CreatureData
            {
                Id = asset.Id,
                Tags = tags,
                DefaultScale = asset.DefaultScale,
                BaseRadius = asset.DefaultScale
            };

            UI_AssetBrowserSetupAssetIndexPatch.AddEntity(asset.Category, entry.GroupTagName, entry, cd,
                asset.ModelCallback);
            if (LogLevel.Value >= ExtraAssetsLibrary.LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:{asset.Id} Added");
        }
    }
}