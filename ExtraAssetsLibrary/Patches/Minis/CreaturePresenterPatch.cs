using System.Collections.Generic;
using DataModel;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.Minis
{
    [HarmonyPatch(typeof(CreaturePresenter), "Present")]
    public class CreaturePresenterPatch
    {
        private static Dictionary<CreatureGuid,UniqueCreatureGuid> cg =
            new Dictionary<CreatureGuid,UniqueCreatureGuid>();

        private static void RemoveCreature()
        {
            foreach (var pair in cg)
            {
                CreatureManager.DeleteCreature(pair.Key, pair.Value);
            }
            cg.Clear();
        }

        private static bool Prefix(in CreatureDataV2 data, ShaderStateRef shaderRef, float time)
        {
            Debug.Log($"{data.CreatureId}");
            Debug.Log($"{data.UniqueId}");
            Debug.Log($"{data.BoardAssetIds[0]}");
            if (!AssetDb.TryGetCreatureData(data.BoardAssetIds[0], out var blobData))
            {
                cg.Add(data.CreatureId, data.UniqueId);
                if (ExtraAssetPlugin.AutoClear.Value) RemoveCreature();
                else SystemMessage.SendSystemMessage("Error", "Creature spawn failed, do you want to remove creature?", "OK", RemoveCreature,"No");
                return false;
            }
            return true;
        }
        
    }
}