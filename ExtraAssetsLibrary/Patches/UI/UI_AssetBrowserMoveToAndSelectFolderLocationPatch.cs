using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "MoveToAndSelectFolderLocation")]
    class UI_AssetBrowserMoveToAndSelectFolderLocationPatch
    {
        public static bool Prefix(ref int folderIndex, ref bool select)
        {
            return true;
        }

    }
}
