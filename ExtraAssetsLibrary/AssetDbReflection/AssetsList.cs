using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LordAshes;
using Newtonsoft.Json;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace PokeSpire.PokeSpire
{
    public partial class AssetsList : Form
    {
        public static AssetBundle myBundle;
        internal static LoadPokemon.pokemons _pokemons;
        public static string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public AssetsList()
        {
            InitializeComponent();
            if (myBundle == null)
            {
                myBundle = AssetBundle.LoadFromFile($"{assemblyFolder}\\pokespire");
                var file = myBundle.LoadAsset<TextAsset>("assets/pokespire/pokemon.json");
                _pokemons = JsonConvert.DeserializeObject<LoadPokemon.pokemons>(file.text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fullname = listBox1.SelectedItem as string;
            var name = fullname.Substring(0, fullname.IndexOf(" ("));
            var Pokemon = _pokemons.Pokemon.Single(p => p.Name.ToLower() == name.ToLower());
            var model = new LoadPokemon.PokemonInstance
            {
                Name = name,
                Shiny = comboBox2.SelectedIndex == 1
            };

            if (fullname.Contains("(Male)")) model.ID = Pokemon.MaleId;
            if (fullname.Contains("(Female)")) model.ID = Pokemon.FemaleID;
            if (fullname.Contains("(Alolan)")) model.ID = Pokemon.Alolan;
            if (fullname.Contains("(MegaX)")) model.ID = Pokemon.MegaID;
            if (fullname.Contains("(MegaY)")) model.ID = Pokemon.MegaYID;

            StatMessaging.SetInfo(new CreatureGuid(PokeSpirePlugin.RadialTargetedMini), PokeSpirePlugin.Guid, JsonConvert.SerializeObject(model));
            Debug.WriteLine(JsonConvert.SerializeObject(model));
            Close();
        }

        private void AssetsList_Load(object sender, EventArgs e)
        {
            // var file = $"{assemblyFolder}\\pokemon.json";
            foreach (var pokemon in _pokemons.Pokemon)
            {
                if (pokemon.MaleId != -1) listBox1.Items.Add($"{pokemon.Name} (Male)");
                if (pokemon.FemaleID != -1) listBox1.Items.Add($"{pokemon.Name} (Female)");
                if (pokemon.Alolan != -1) listBox1.Items.Add($"{pokemon.Name} (Alolan)");
                if (pokemon.MegaID != -1) listBox1.Items.Add($"{pokemon.Name} (MegaX)");
                if (pokemon.MegaYID != -1) listBox1.Items.Add($"{pokemon.Name} (MegaY)");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var text = comboBox1.SelectedText;
            if (text == "All")
                foreach (var pokemon in _pokemons.Pokemon)
                {
                    if (pokemon.MaleId != -1) listBox1.Items.Add($"{pokemon.Name} (Male)");
                    if (pokemon.FemaleID != -1) listBox1.Items.Add($"{pokemon.Name} (Female)");
                    if (pokemon.Alolan != -1) listBox1.Items.Add($"{pokemon.Name} (Alolan)");
                    if (pokemon.MegaID != -1) listBox1.Items.Add($"{pokemon.Name} (MegaX)");
                    if (pokemon.MegaYID != -1) listBox1.Items.Add($"{pokemon.Name} (MegaY)");
                }
            else
                foreach (var pokemon in _pokemons.Pokemon)
                {
                    if (pokemon.MaleId != -1 && text == "Male") listBox1.Items.Add($"{pokemon.Name} (Male)");
                    if (pokemon.FemaleID != -1 && text == "Female") listBox1.Items.Add($"{pokemon.Name} (Female)");
                    if (pokemon.Alolan != -1 && text == "Alolan") listBox1.Items.Add($"{pokemon.Name} (Alolan)");
                    if (pokemon.MegaID != -1 && text == "MegaX") listBox1.Items.Add($"{pokemon.Name} (MegaX)");
                    if (pokemon.MegaYID != -1 && text == "MegaY") listBox1.Items.Add($"{pokemon.Name} (MegaY)");
                }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
