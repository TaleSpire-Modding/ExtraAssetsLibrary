using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(CreatureSpawnerBoardTool), "SwitchCreatureTool")]
    public class CreatureSpawnBoardAssetSpawnPatch
    {
        private static void Postfix(CreatureDataV2 info, ref CreaturePreviewBoardAsset ____pickupObject)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(____pickupObject.DbEntry.Id))
            {
                var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____pickupObject.DbEntry.Id];
                ____pickupObject.Scale = asset.DefaultScale;
            }


            if (AssetLoaderInitPatch.stopSpawn)
            {
                Debug.Log("Extra Asset Library Plugin:Closing Spawner");
                AssetLoaderInitPatch.stopSpawn = false;
                SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
            }
        }
    }
}