using System.Collections.Generic;
using ExtraAssetsLibrary.AssetDbExtension;
using ExtraAssetsLibrary.DTO;
using ExtraAssetsLibrary.Handlers;
using HarmonyLib;
using LordAshes;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "SwitchCatagory")]
    class UI_AssetBrowserSwitchCatagoryPatch
    {
        public static bool Prefix(ref int index, ref UI_AssetBrowser __instance,AssetBrowserSearch ____search)
        {
            if (index > 2)
            {
                __instance.call("SetupCategory", index);
                switch (index)
                {
                    case 3:
                        ____search.SwitchAssetKind((AssetDb.DbEntry.EntryKind) CustomEntryKind.Aura);
                        break;
                    case 4:
                        ____search.SwitchAssetKind((AssetDb.DbEntry.EntryKind) CustomEntryKind.Slab);
                        break;
                    case 5:
                        ____search.SwitchAssetKind((AssetDb.DbEntry.EntryKind) CustomEntryKind.Audio);
                        break;
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UI_SwitchButtonGroup), "Start")]
    class UI_SwitchButtonGroupStartPatch
    {
        private static void Add(UI_SwitchButtonGroup __instance, int i, Button template, string key, ref List<Button> ____buttons)
        {
            var distance = 30f;
            var clone = Object.Instantiate(template);
            clone.transform.SetParent(__instance.transform, false);
            ____buttons.Add(clone);
            clone.gameObject.name = key;
            var newPost = new Vector3(clone.transform.position.x, clone.transform.position.y - (distance * i), clone.transform.position.z);
            var rot = clone.transform.rotation;
            clone.transform.SetPositionAndRotation(newPost, rot);
            switch (i)
            {
                case 3:
                    clone.GetComponentsInChildren<Image>()[2].sprite = FileAccessPlugin.Image.LoadSprite("Images/Icons/aura-and-effects-fire.png");
                    break;
                case 4:
                    clone.GetComponentsInChildren<Image>()[2].sprite = FileAccessPlugin.Image.LoadSprite("Images/Icons/slab-sketch.png");
                    break;
                case 5:
                    clone.GetComponentsInChildren<Image>()[2].sprite = FileAccessPlugin.Image.LoadSprite("Images/Icons/audio-note.png");
                    break;
            }
        }

        public static void Prefix(UI_SwitchButtonGroup __instance, 
            ref List<Button> ____buttons)
        {
            Debug.Log($"UI_SwitchButtonGroup Count:{____buttons.Count}");
            Debug.Log($"UI_SwitchButtonGroup Name:{__instance.gameObject.name}");
            if (__instance.gameObject.name == "Catagory")
            {
                var template = ____buttons[0];
                Add(__instance,3,template,"Aura and Effects",ref ____buttons);
                Add(__instance,4,template,"Slabs",ref ____buttons);
                Add(__instance,5,template,"Audio",ref ____buttons);
            }
        }
    }
}
