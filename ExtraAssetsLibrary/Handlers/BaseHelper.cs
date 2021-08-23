using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bounce.Unmanaged;
using ExtraAssetsLibrary.Patches;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.Handlers
{
    public static class BaseHelper
    {
        /// <summary>
        /// Need to adjust and point this towards TaleSpire.exe
        /// </summary>
        private static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Cache of bases as needed.
        /// </summary>
        // public static Dictionary<string, GameObject> _loadedBases = new Dictionary<string, GameObject>();

        /// <summary>
        /// Provides an empty gameobject as a template for a creature that wants no base.
        /// </summary>
        /// <param name="nguid">ID of creature using the base</param>
        /// <returns>non-initialized gameobject template for base</returns>
        public static GameObject NoBase(NGuid nguid = new NGuid()) => new GameObject();

        /// <summary>
        /// This is needs to be updated to point towards actual file and not temp copy
        /// </summary>
        /// <param name="nguid">ID of creature using the base</param>
        /// <returns>non-initialized gameobject template for base</returns>
        public static GameObject DefaultBase(NGuid nguid = new NGuid())
        {
            if (AssetLoadManager.Instance.TryGetAsset("d71427a1-5535-4fa7-82d7-4ca1e75edbfdchar_base01_1462710208clothBase", out var reference) == AssetLoadManager.AssetLoadStatus.Loaded) return reference;

            string id = "DefaultClothBase";
            // if (_loadedBases.ContainsKey(id)) return _loadedBases[id];

            var myBundle = AssetBundle.LoadFromFile($"{assemblyFolder}\\default_base");
            var miniBase = myBundle.LoadAsset<GameObject>("clothBase");
            var baseRenderer = miniBase.GetComponent<Renderer>();
            baseRenderer.material.shader = Shader.Find("Taleweaver/CreatureShader");
            foreach (var renderer in miniBase.GetComponentsInChildren<Renderer>())
            {
                renderer.material.shader = Shader.Find("Taleweaver/CreatureShader");
            }
            // _loadedBases[id] = miniBase;

            AssetLoadManagerInjectGameObjectAsAssetPatch.InjectGameObjectAsAssetPatch(
                miniBase,
                float3.zero,
                new quaternion(0, 0, 0, 0),
                new float3(1, 1, 1),
                new NGuid("d71427a1-5535-4fa7-82d7-4ca1e75edbfd"),
                new NGuid("d71427a1-5535-4fa7-82d7-4ca1e75edbfd"),
                "d71427a1-5535-4fa7-82d7-4ca1e75edbfdchar_base01_1462710208clothBase"
            );

            return miniBase;
        }
    }
}
