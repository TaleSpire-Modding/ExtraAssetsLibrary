using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary;
using ExtraAssetsLibrary.Handlers;
using ExtraAssetsLibrary.Patches;
using ExtraAssetsLibrary.Patches.Projectile;
using HarmonyLib;
using LordAshes;
using RadialUI;
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
    [BepInDependency(RadialUIPlugin.Guid)]
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

        internal static Dictionary<string, Action<CustomEntryKind>> OnCatagoryChange =
            new Dictionary<string, Action<CustomEntryKind>>();

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
            DoConfig(Config);
            if (LogLevel.Value > ExtraAssetsLibrary.LogLevel.None) Debug.Log($"Extra Asset Library Plugin:{Name} is Active.");
            UI_AssetBrowserSetupAssetIndexPatch.initStatic();
            DoPatching();
            
        }

        internal static bool ClothBaseLoaded;
        internal static bool Reloaded;

        private void Update()
        {
            if (OnBoard())
            {
                if(!ClothBaseLoaded) AssertClothBaseLoaded();
                else if (!Reloaded)
                {
                    var _info = BoardSessionManager.CurrentBoardInfo;
                    CampaignSessionManager.LoadBoard(_info);
                    Reloaded = true;
                }
            }
        }

        private void AssertClothBaseLoaded()
        {
            var command = $"talespire://asset/32fbdb43-e809-4eea-a834-5d120886bd81";
            System.Diagnostics.Process.Start(command).WaitForExit();
            SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
            BaseHelper.DefaultBase();
        }

        private bool OnBoard()
        {
            return (CameraController.HasInstance &&
                    BoardSessionManager.HasInstance &&
                    BoardSessionManager.HasBoardAndIsInNominalState &&
                    !BoardSessionManager.IsLoading);
        }

        /// <summary>
        ///     This method should be run on awake by your plugin.
        /// </summary>
        /// <param name="audio">Description of your audio you are injecting.</param>
        private static void AddAudio(Audio audio)
        {
        }

        public static void AddOnCatagoryChange(string Guid, Action<CustomEntryKind> Callback)
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
                RadialUIPlugin.AddCustomButtonAttacksSubmenu(Guid, new MapMenu.ItemArgs
                    {
                        Action = ParticleStack.CustomParticle,
                        Title = asset.Name,
                        Icon = asset.Icon,
                        Obj = asset.ModelCallback
                    }, ParticleStack.Check
                );
                return;
            }

            if (LogLevel.Value >= ExtraAssetsLibrary.LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:Adding: {asset.Id}");
            if (!UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(asset.Id))
                UI_AssetBrowserSetupAssetIndexPatch.assets.Add(asset.Id, asset);
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
                BaseRadius = asset.DefaultScale
            };

            if (asset.BaseCallback != null) UI_AssetBrowserSetupAssetIndexPatch.Bases.Add(asset.Id, asset.BaseCallback);
            UI_AssetBrowserSetupAssetIndexPatch.AddEntity(asset.Kind, entry.GroupTagName, entry, cd,
                asset.ModelCallback);
            if (LogLevel.Value >= ExtraAssetsLibrary.LogLevel.Medium) Debug.Log($"Extra Asset Library Plugin:{asset.Id} Added");
        }
    }
}