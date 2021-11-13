using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "ListClickCallback")]
    class UI_AssetBrowserListClickCallbackPatch
    {
        public static bool Prefix(UIListItemClickEvents obj)
        {
            Debug.Log("ListClickCallback Starting");
            return true;
        }
    }
}
