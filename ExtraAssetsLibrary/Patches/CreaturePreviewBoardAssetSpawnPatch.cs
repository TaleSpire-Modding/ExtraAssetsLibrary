using UnityEngine;
using HarmonyLib;
using Unity.Mathematics;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(CreatureSpawnerBoardTool), "SwitchCreatureTool")]
    public class CreaturePreviewBoardAssetSpawnPatch
    {

        static void Postfix(CreatureDataV2 info)
        {
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
