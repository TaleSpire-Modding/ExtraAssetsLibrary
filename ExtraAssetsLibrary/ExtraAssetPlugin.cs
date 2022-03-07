using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Patches;
using HarmonyLib;
using LordAshes;
using Newtonsoft.Json;
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
        public const string Version = "2.0.0.0";
        private const string Name = "HolloFoxes' Extra Asset Library";

        internal static ConfigEntry<bool> AutoClear { get; set; }
        internal static ConfigEntry<LogLevel> LogLevel { get; set; }
        private static ConfigEntry<string> _hiddenGroups { get; set; }
        internal static Dictionary<NGuid, Asset> RegisteredAssets { get; set; } = new Dictionary<NGuid, Asset>();

        public static List<string> HiddenGroups
        {
            get => JsonConvert.DeserializeObject<List<string>>(_hiddenGroups.Value);
            set => _hiddenGroups.Value = JsonConvert.SerializeObject(value);
        }

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
            if (LogLevel.Value > ExtraAssetsLibrary.LogLevel.None) Debug.Log($"Extra Asset Library Plugin: Patched.");
        }

        public static void DoConfig(ConfigFile Config)
        {
            AutoClear = Config.Bind("Mini Loading", "Auto Clear Failed Minis", false);
            LogLevel = Config.Bind("Logging", "Level", ExtraAssetsLibrary.LogLevel.Low);
            _hiddenGroups = Config.Bind("Groups", "Hidden", JsonConvert.SerializeObject(new List<string>
            {
                "[variants]"
            }));
            if (LogLevel.Value > ExtraAssetsLibrary.LogLevel.None) Debug.Log($"Extra Asset Library Plugin: Config Bound.");
        }

        private void Awake()
        {
            DoConfig(Config);
            DoPatching();
            if (LogLevel.Value > ExtraAssetsLibrary.LogLevel.None) Debug.Log($"Extra Asset Library Plugin:{Name} is Active.");
        }

        internal static bool ClothBaseLoaded;


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

            RegisteredAssets.Add(asset.Id, asset);

            switch (asset.Category)
            {
                case Category.Creature:
                    AssetDbOnSetupInternalsPatch.InjectCreature(asset);
                    break;
                case Category.Prop:
                    AssetDbOnSetupInternalsPatch.InjectProps(asset);
                    break;
                case Category.Tile:
                    AssetDbOnSetupInternalsPatch.InjectTiles(asset);
                    break;
                case Category.AuraAndEffects:
                    // AssetDbOnSetupInternalsPatch.InjectAura(asset);
                    break;
                case Category.Slab:
                    // AssetDbOnSetupInternalsPatch.InjectSlab(asset);
                    break;
                case Category.Audio:
                    // AssetDbOnSetupInternalsPatch.InjectAudio(asset);
                    break;
                default:
                    AssetDbOnSetupInternalsPatch.InjectCreature(asset);
                    break;
            }
            if (LogLevel.Value == ExtraAssetsLibrary.LogLevel.All) Debug.Log($"Extra Asset Library Plugin: Registered Asset {asset.Name}");
        }
    }
}