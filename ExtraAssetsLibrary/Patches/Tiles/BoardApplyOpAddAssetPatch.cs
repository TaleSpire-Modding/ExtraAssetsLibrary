using DataModel;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(Board), "ApplyOp", typeof(MessageInfo), typeof(ClientGuid), typeof(AddAssetOp))]
    internal class BoardApplyOpAddAssetPatch
    {
        private static bool Prefix(MessageInfo info, ClientGuid hostId, AddAssetOp op)
        {
            if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log("Extra Asset Library Plugin:Loading Object");
            if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log($"Extra Asset Library Plugin:ID:{op.AssetId}");
            return true;
        }
    }
}