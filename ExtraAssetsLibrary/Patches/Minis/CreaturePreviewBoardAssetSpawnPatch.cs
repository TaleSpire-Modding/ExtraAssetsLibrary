using System.Windows.Forms.VisualStyles;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{

    // [HarmonyPatch(typeof(CreatureSpawnerBoardTool), "SwitchCreatureTool")]
    public class CreatureSpawnBoardAssetSpawnPatch
    { 
        internal static bool respawn;
        internal static CreatureDataV2? tinfo;

        private static void Prefix(CreatureDataV2 info)
        {
            tinfo = info;
        }

        private static void Postfix(CreatureDataV2 info, ref CreaturePreviewBoardAsset ____pickupObject)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(____pickupObject.DbEntry.Id))
            {
                var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____pickupObject.DbEntry.Id];
                ____pickupObject.Scale = asset.DefaultScale;
            }
        }
    }
}