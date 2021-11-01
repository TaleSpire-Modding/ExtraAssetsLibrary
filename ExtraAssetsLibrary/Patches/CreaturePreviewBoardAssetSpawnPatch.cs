using Bounce.TaleSpire.AssetManagement;
using UnityEngine;
using HarmonyLib;
using Unity.Mathematics;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(CreatureSpawnerBoardTool), "SwitchCreatureTool")]
    public class CreaturePreviewBoardAssetSpawnPatch
    {
        static void Postfix(CreatureDataV2 info, ref CreaturePreviewBoardAsset ____pickupObject)
        {
           
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(____pickupObject.DbEntry.Id))
            {
                CreatureManager.SetCreatureScale(____pickupObject.CreatureData.CreatureId, 0,2);
                Debug.Log($"found:{____pickupObject.DbEntry.Id}");
                Debug.Log($"Scale:{____pickupObject.BaseRadius}");
                Debug.Log($"Scale:{____pickupObject.Scale}");
            }

            
            if (AssetLoaderInitPatch.stopSpawn)
            {
                Debug.Log("Closing Spawner");
                AssetLoaderInitPatch.stopSpawn = false;
                SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
            }
            else
            {
                Debug.Log("Postfix ran");
            }
        }
    }
}
