using Bounce.Unmanaged;
using ExtraAssetsLibrary.DTO;
using UnityEngine;
using HarmonyLib;
using Unity.Mathematics;

namespace ExtraAssetsLibrary.Patches.Minis
{
    [HarmonyPatch(typeof(UI_AssetBrowserSlotItem), "Spawn")]
    public class UI_AssetBrowserSlotItemSpawnPatch
    {
        static bool Prefix(NGuid ____nGuid,AssetDb.DbEntry.EntryKind ____entityKind)
        {
            Debug.Log($"Entry Kind:{(CustomEntryKind)____entityKind}");
            var pass = true;
            foreach (var action in ExtraAssetPlugin.CoreAssetPrefixCallbacks.Values)
            {
                pass = pass && action.Invoke(____nGuid,____entityKind);
            }
            Debug.Log($"Extra Asset Library Plugin: CoreAssetPrefixCallbacks: {pass}");

            if (pass){
                if ((CustomEntryKind)____entityKind == CustomEntryKind.Aura)
                {
                    if (SlotItemSpawnPatch(____nGuid))
                    {
                        Debug.Log($"Extra Asset Library Plugin: Aura being called");
                        var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____nGuid];
                        asset.ModelCallback(____nGuid);
                    }
                }
                else if ((CustomEntryKind) ____entityKind == CustomEntryKind.Effects)
                {
                    if (SlotItemSpawnPatch(____nGuid))
                    {
                        Debug.Log($"Extra Asset Library Plugin: Effects being called");
                        var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____nGuid];
                        asset.ModelCallback(____nGuid);
                    }
                }
                else if ((CustomEntryKind)____entityKind == CustomEntryKind.Slab)
                {
                    if (SlotItemSpawnPatch(____nGuid))
                    {
                        Debug.Log($"Extra Asset Library Plugin: Slab being called");
                        var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____nGuid];
                        asset.ModelCallback(____nGuid);
                    }
                }
                else if ((CustomEntryKind)____entityKind == CustomEntryKind.Audio)
                {
                    if (SlotItemSpawnPatch(____nGuid))
                    {
                        Debug.Log($"Extra Asset Library Plugin: Audio being called");
                        var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[____nGuid];
                        asset.ModelCallback(____nGuid);
                    }
                }
            }

            return pass;
        }

        internal static bool SlotItemSpawnPatch(NGuid id)
        {
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(id))
            {
                var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[id];
                var canLoad = asset.PreCallback == null || asset.PreCallback(id);
                Debug.Log($"Extra Asset Library Plugin: Pre-callback called and value: {canLoad}");
                return canLoad;
            }
            Debug.Log($"Extra Asset Library Plugin: Core Asset loaded into Spawner");
            return true;

            
        }
    }

    [HarmonyPatch(typeof(UI_AssetBrowserSlotItem), "SpawnCreature")]
    public class UI_AssetBrowserSlotItemSpawnCreaturePatch
    {
        static bool Prefix(NGuid ____nGuid)
        {
            return UI_AssetBrowserSlotItemSpawnPatch.SlotItemSpawnPatch(____nGuid);
        }
    }

    [HarmonyPatch(typeof(UI_AssetBrowserSlotItem), "SpawnTile")]
    public class UI_AssetBrowserSlotItemSpawnTilePatch
    {
        static bool Prefix(NGuid ____nGuid)
        {
            return UI_AssetBrowserSlotItemSpawnPatch.SlotItemSpawnPatch(____nGuid);
        }
    }

    [HarmonyPatch(typeof(UI_AssetBrowserSlotItem), "SpawnProp")]
    public class UI_AssetBrowserSlotItemSpawnPropPatch
    {
        static bool Prefix(NGuid ____nGuid)
        {
            return UI_AssetBrowserSlotItemSpawnPatch.SlotItemSpawnPatch(____nGuid);
        }
    }

    [HarmonyPatch(typeof(CreatureManager), "SetLocationData")]
    public class PatchLocationData
    {
        static bool Prefix()
        {
            Debug.Log($"Extra Asset Library Plugin: SetLocationData Triggered");
            return true;
        }
    }

    [HarmonyPatch(typeof(CreatureBoardAsset), "InitCommon")]
    public class PatchCreatureBoardAsset
    {
        static bool Prefix(
            ref CreatureDataV2 creatureData,
            ref float3 headPos,
            ref float3 torchPos,
            ref float3 spellPos,
            ref float3 hitPos,
            ref float baseRadius,
            ref float height,
            ref float defaultScale)
        {
            var id = creatureData.BoardAssetIds[0];
            if (UI_AssetBrowserSetupAssetIndexPatch.assets.ContainsKey(id))
            {
                var asset = UI_AssetBrowserSetupAssetIndexPatch.assets[id];
                defaultScale = 1;// asset.DefaultScale;
                torchPos = asset.torchPos;
                headPos = asset.headPos;
                spellPos = asset.spellPos;
                hitPos = asset.hitPos;
                Debug.Log($"Extra Asset Library Plugin: SetLocationData Triggered");
            }
            return true;
        }
    }

}
