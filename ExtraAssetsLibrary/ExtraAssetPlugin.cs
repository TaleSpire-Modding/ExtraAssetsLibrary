using BepInEx;
using Bounce.TaleSpire.AssetManagement;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using ExtraAssetsLibrary.Patches;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ExtraAssetsLibrary
{
    [BepInPlugin(Guid, Name, Version)]
    public class ExtraAssetPlugin:BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.TMC.plugins.ExtraAssetLib";
        public const string Version = "1.0.1.0";
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
        /// <param name="asset">Description of your asset you are injecting.</param>
        public static void AddAsset(Asset asset)
        {
            var group = UI_AssetBrowserSetupAssetIndexPatch.AddGroup(asset.Kind, asset.GroupName);
            var tags = BlobHandler.ConstructBlobData(new string[] { });
            var entry = new AssetDb.DbEntry(asset.Id, asset.Kind, asset.Name,
                asset.Description, group, group.Name, asset.groupTagOrder, ref tags, asset.isDeprecated, asset.Icon);

            var cd = new CreatureData
            {
                Id = asset.Id,
                // ModelAsset = *modelAsset.Value,
                DefaultScale = asset.DefaultScale,
            };

            if (asset.BaseCallback != null) UI_AssetBrowserSetupAssetIndexPatch.Bases.Add(asset.Id,asset.BaseCallback);
            UI_AssetBrowserSetupAssetIndexPatch.AddEntity(asset.Kind,entry.GroupTagName,entry,cd,asset.ModelCallback);

            if (FindObjectOfType<UI_AssetBrowser>() != null)
            {
                // Adding to existing UI
                var UI = FindObjectOfType<UI_AssetBrowser>();
                MethodInfo dynMethod = typeof(UI_AssetBrowser).GetMethod("Start",bindFlags);
                dynMethod.Invoke(UI, new object[] { });
            }
        }
    }
}
