using System.IO;
using LordAshes;
using Newtonsoft.Json;
using SRF;
using UnityEngine;

namespace PokeSpire.PokeSpire
{
    public static class PokeSpireStatHandler
    {

        public static void LoadCustomContent(CreatureBoardAsset asset, string source)
        { 
            Debug.Log("Received PokeSpire Message:");
            Debug.Log(source);
            
            var rootPath = PokeSpirePlugin.PokeOneDirectory.Value;
            var data = JsonConvert.DeserializeObject<LoadPokemon.PokemonInstance>(source);
            var handler = CustomMiniPlugin.GetRequestHander();
            Debug.Log($"{rootPath}\\files\\PokeOne_Data\\StreamingAssets\\psdata");
            
            if (Directory.Exists($"{rootPath}\\files\\PokeOne_Data\\StreamingAssets"))
            {
                Debug.Log("Exists");
                /* handler.LoadCustomContent(asset, CustomMiniPlugin.RequestHandler.LoadType.mini ,
                    LoadPokemon.GetSource(data.ID), LoadPokemon.GetPrefab(data.ID));
                */
                var myLoadedAssetBundle = AssetBundle.LoadFromFile(LoadPokemon.GetSource(data.ID));
                var prefab =
                    myLoadedAssetBundle.LoadAsset<GameObject>(LoadPokemon.GetPrefab(data.ID));
                var obj= Object.Instantiate(prefab);

                foreach (var child in obj.GetComponentsInChildren<Transform>())
                {
                    child.gameObject.AddComponent<PokeShaderReset>();
                }

                obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

                var inst = obj;
                var animation = inst.GetComponentInChildren<Animation>().gameObject;
                if (!animation.TryGetComponent(typeof(PokeAnimControl), out _))
                {
                    animation.AddComponent<PokeAnimControl>();
                }
                LoadPokemon.PokemonCry(animation, data.ID);
            } else Debug.Log("Does not Exist");
        }
    }
}
