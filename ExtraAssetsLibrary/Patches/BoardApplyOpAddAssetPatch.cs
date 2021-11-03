using DataModel;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(Board), "ApplyOp",typeof(MessageInfo), typeof(ClientGuid), typeof(AddAssetOp))]
    class BoardApplyOpAddAssetPatch
    {
        static bool Prefix(MessageInfo info, ClientGuid hostId, AddAssetOp op)
        {
            Debug.Log("Extra Asset Library Plugin:Loading Object");
            Debug.Log($"Extra Asset Library Plugin:ID:{op.AssetId}");
            return true;
        }
    }
}
