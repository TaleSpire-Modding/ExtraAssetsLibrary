using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Bounce.Unmanaged;
using LordAshes;
using Newtonsoft.Json;
using RadialUI;
using UnityEngine;

namespace PokeSpire.PokeSpire
{

    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(CustomMiniPlugin.Guid)]
    [BepInDependency(StatMessaging.Guid)]
    [BepInDependency(RadialUIPlugin.Guid)]
    // [BepInDependency(PatreonPlugin.Guid)] // Patreon Plugin
    public class PokeSpirePlugin: BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.PokeSpirePlugin";
        public const string Version = "0.9.0.0";
        private const string Name = "HolloFoxes' PokeSpire";

        public static ConfigEntry<string> PokeOneDirectory { get; set; }

        // Need to remove these and use SystemMessageExtensions
        private AssetsList list;
        private AssetsList effectsList;

        // My StatHandler
        private static bool ready = false;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log($"{Name} is Active.");

            PokeOneDirectory = Config.Bind("PokeOne", "Directory", "C:\\Program Files (x86)\\Steam\\steamapps\\common\\PokeOne");

            ModdingUtils.Initialize(this, Logger);

            StatMessaging.Subscribe(Guid, Request);

            BoardSessionManager.OnStateChange += (s) =>
            {
                if (s.ToString().Contains("+Active"))
                {
                    ready = true;
                    Debug.Log("Stat Messaging started looking for messages.");
                }
                else
                {
                    ready = false;
                    StatMessaging.Reset();
                    Debug.Log("Stat Messaging stopped looking for messages.");
                }
            };

            /*
            RadialUIPlugin.AddOnCharacter(Guid + "SetAuras",
                new MapMenu.ItemArgs
                {
                    Title = "Set Auras",
                    CloseMenuOnActivate = true,
                    Action = AddAura,
                    Icon = sprite("Aura.png")
                },
                IsInGmMode
            );
            */

            RadialUIPlugin.AddOnCharacter(Guid + "RevertMini",
                new MapMenu.ItemArgs
                {
                    Title = "Revert Mini",
                    CloseMenuOnActivate = true,
                    Action = RevertMini
                },
                HasChanged
            );

            RadialUIPlugin.AddOnCharacter(Guid + "ChangeMini",
                new MapMenu.ItemArgs
                {
                    Title = "Set Pokemon",
                    CloseMenuOnActivate = true,
                    Action = ToggleMini
                },
                IsInGmMode
                );
        }

        private static Sprite sprite(string FileName)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return RadialSubmenu.GetIconFromFile(dir + "\\" + FileName);
        }

        public void Request(StatMessaging.Change[] changes)
        {
            // Process all changes
            foreach (StatMessaging.Change change in changes)
            {
                // Find a reference to the indicated mini
                CreaturePresenter.TryGetAsset(change.cid, out var asset);
                if (asset != null)
                {

                    PokeSpireStatHandler.LoadCustomContent(asset, change.value);
                }
            }
        }

        private static bool ShowMenu = false;

        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F3))
            {
                RadialTargetedMini = LocalClient.SelectedCreatureId.Value;
                ShowMenu = true;

            }
            if (ShowMenu)
            {
                ChangeMini();
                ShowMenu = false;
            }
        }

        public void ToggleMini(MapMenuItem mmi, object o)
        {
            ShowMenu = true;
        }

        public void ChangeMini()
        {
            if (list == null || list.IsDisposed) list = new AssetsList();
            list.Show();
            list.Focus();
        }

        public void RevertMini(MapMenuItem mmi, object o)
        {
            Debug.Log("Minis Called");
            if (list == null || list.IsDisposed) list = new AssetsList();
            list.Show();
        }

        public static NGuid RadialTargetedMini;

        private static bool IsInGmMode(NGuid selected, NGuid targeted)
        {
            RadialTargetedMini = targeted;
            return LocalClient.IsInGmMode;
        }

        private static bool HasChanged(NGuid selected, NGuid targeted)
        {
            RadialTargetedMini = targeted;
            return false; // LocalClient.IsInGmMode;
        }
    }
}
