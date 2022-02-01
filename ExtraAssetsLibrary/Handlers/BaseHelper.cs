using System.IO;
using BepInEx;
using Bounce.Unmanaged;
using UnityEngine;

namespace ExtraAssetsLibrary.Handlers
{
    public static class BaseHelper
    {
        /// <summary>
        ///     Need to adjust and point this towards TaleSpire.exe
        /// </summary>
        private static string assemblyFolder = $"{Path.GetDirectoryName(Paths.ExecutablePath)}";

        /// <summary>
        ///     Provides an empty gameobject as a template for a creature that wants no base.
        /// </summary>
        /// <param name="nguid">ID of creature using the base</param>
        /// <returns>non-initialized gameobject template for base</returns>
        public static GameObject NoBase(NGuid nguid = new NGuid())
        {
            return new GameObject();
        }

        /// <summary>
        ///     This is needs to be updated to point towards actual file and not temp copy
        /// </summary>
        /// <param name="nguid">ID of creature using the base</param>
        /// <returns>non-initialized gameobject template for base</returns>
        public static GameObject DefaultBase(NGuid nguid = new NGuid())
        {
            if (AssetLoadManager.Instance.TryGetAsset(
                    "d71427a1-5535-4fa7-82d7-4ca1e75edbfdchar_base01_1462710208clothBase", out var reference) ==
                AssetLoadManager.AssetLoadStatus.Loaded)
            {
                ExtraAssetPlugin.ClothBaseLoaded = true;
                return reference;
            }
            return new GameObject();
        }

        public static bool BaseIsLoaded() => ExtraAssetPlugin.ClothBaseLoaded;
    }
}