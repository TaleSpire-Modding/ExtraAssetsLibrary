using HarmonyLib;

namespace ExtraAssetsLibrary.Patches.Tiles
{
    [HarmonyPatch(typeof(PlaceableManager), "LoadPlaceablePrototype")]
    internal class PalceableManagerLoadPlaceablePrototypePatch
    {
        public static bool Prefix()
        {
            return true;
        }
    }
}