using Newtonsoft.Json;
using UnityEngine;

namespace PokeSpire.PokeSpire
{
    public class PokeShaderReset : MonoBehaviour
    {
        public static Shader AssetShader;

        // Start is called before the first frame update
        void Start()
        {
            if (AssetsList.myBundle == null)
            {
                AssetsList.myBundle = AssetBundle.LoadFromFile($"{AssetsList.assemblyFolder}\\pokespire");
                var file = AssetsList.myBundle.LoadAsset<TextAsset>("assets/pokespire/pokemon.json");
                AssetsList._pokemons = JsonConvert.DeserializeObject<LoadPokemon.pokemons>(file.text);
            }

            if (AssetShader == null)
            {
                AssetShader = AssetsList.myBundle.LoadAsset<Shader>("assets/pokespire/pokeshader.shader");
            }

            var renderer = GetComponent<Renderer>();
            Debug.Log($"Renderer Found: {renderer.name}");
            Debug.Log($"Material Found: {renderer.material.name}");

            var tex1 = renderer.material.GetTexture("_Texture0");
            var tex2 = renderer.material.GetTexture("_Texture1");
            var tex3 = renderer.material.GetTexture("_Texture2");

            tex1.wrapModeU = TextureWrapMode.Mirror;
            tex2.wrapModeU = TextureWrapMode.Mirror;
            tex3.wrapModeU = TextureWrapMode.Mirror;

            var v1 = renderer.material.GetVector("_Texture0_ST");
            var v2 = renderer.material.GetVector("_Texture1_ST");
            var v3 = renderer.material.GetVector("_Texture2_ST");

            renderer.material.shader = AssetShader;
            // renderer.material.shader = Shader.Find("Custom/PokemonShader");

            renderer.material.mainTexture = tex1;
            renderer.material.SetTexture("_BumpMap", tex2);
            renderer.material.SetTexture("_LightMap", tex3);

            renderer.material.SetVector("_MainTex_ST", v1);
            renderer.material.SetVector("_BumpMap_ST", v2);
            renderer.material.SetVector("_LightMap_ST", v3);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }

}
