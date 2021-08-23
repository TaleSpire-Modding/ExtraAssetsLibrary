using System;
using UnityEngine;

namespace PokeSpire.PokeSpire
{
    public class LoadPokemon : MonoBehaviour
    {
        // Config
        private static readonly string rootPath = PokeSpirePlugin.PokeOneDirectory.Value;

        // Start is called before the first frame update
        void Start()
        {
        }


        private Texture2D FetchPokemonSprite(int id)
        {
            string path = $"{rootPath}\\files\\PokeOne_Data\\StreamingAssets\\psdata";
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
            return myLoadedAssetBundle.LoadAsset<Texture2D>($"assets/assetbundles/pokemonsprites/small/{id}.png");
        }

        public static string GetSource(int id)
        {
            float pack = (float)Math.Floor(id / 100f) + 1;
            return $"{rootPath}\\files\\PokeOne_Data\\StreamingAssets\\pdata{pack}";
        }

        public static string GetPrefab(int id)
        {
            float pack = (float)Math.Floor(id / 100f) + 1;
            return $"assets/assetbundles/pokes{pack}/{id}/model.prefab";
        }

        /*private GameObject SpawnPokemon(int pokeId, Model model = Model.Male)
        {
            GameObject obj = null;
            var pokemon = _pokemons.Pokemon.SingleOrDefault(p => p.ID == pokeId);
            if (pokemon != null)
            {
                var id = model == Model.Female ? pokemon.FemaleID : pokemon.MaleId;
                if (model == Model.Alolan && pokemon.Alolan != -1) id = pokemon.Alolan;
                else if (model == Model.MegaX && pokemon.MegaID != -1) id = pokemon.MegaID;
                else if (model == Model.MegaY && pokemon.MegaYID != -1) id = pokemon.MegaYID;

                float pack = (float)Math.Floor(id / 100f) + 1;
                string path = $"{rootPath}\\files\\PokeOne_Data\\StreamingAssets\\pdata{pack}";

                myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
                var prefab =
                    myLoadedAssetBundle.LoadAsset<GameObject>($"assets/assetbundles/pokes{pack}/{id}/model.prefab");

                obj = Instantiate(prefab) as GameObject;

                if (model == Model.MegaX || model == Model.MegaY)
                {
                    obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f) * pokemon.ScaleFactorMega;
                }
                else
                {
                    obj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f) * pokemon.ScaleFactor;
                }

                AnimControl = obj.AddComponent<PokeAnimControl>();
            }

            return obj;
        }
        */


        public static void PokemonCry(GameObject inst, int pokeId)
        {
            string path = $"{rootPath}\\files\\PokeOne_Data\\StreamingAssets\\cdata";
            var assetBundle = AssetBundle.LoadFromFile(path);
            var id = "00" + pokeId;
            id = id.Substring(id.Length - 3);
            var prefab = assetBundle.LoadAsset<AudioClip>($"assets/assetbundles/crys/{id}.wav");
            if (!inst.TryGetComponent(out AudioSource audio)) audio = inst.AddComponent<AudioSource>();
            audio.clip = prefab;
            audio.volume = 0.1f;
            audio.Play();
        }

        // Update is called once per frame
        void Update()
        {

        }

        internal class pokemons
        {
            public Pokemon[] Pokemon = new Pokemon[800];
        }

        internal class Pokemon
        {
            public int ID;
            public string Name;
            public int MaleId = -1;
            public int FemaleID = -1;
            public int MegaID = -1;
            public int MegaYID = -1;
            public float ScaleFactor = 1;
            public float ScaleFactorMega = 1;
            public int Alolan = -1;
        }

        internal class PokemonInstance
        {
            public int ID;
            public string Name;
            public bool Shiny;
        }
    }

}