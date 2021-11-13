using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ExtraAssetsLibrary.Patches.Tiles
{
    [HarmonyPatch(typeof(PlaceableManager), "LoadPlaceablePrototype")]
    class PalceableManagerLoadPlaceablePrototypePatch
    {
        public static bool Prefix()
        {
            return true;
        }
    }
}
