## Extra Assets Library
The Extra Assets Library is a plugin to register custom assets into TaleSpire. 

## Installing With R2ModMan
This package is designed specifically for R2ModMan and Talespire. 
You can install them via clicking on "Install with Mod Manager" or using the r2modman directly.

## Installing Without R2ModMan (WIP)
You can still manually install this with the downloaded DLLs.
Upon doing so, you'll need to configure the plugin manual by updating the config file.

## Player Usage
Players will start to find all your minis within the UI Asset Browser with every other minis.

## Writing Mods with this

Loading an asset in:
```CSharp
var asset = new ExtraAssetsLibrary.DTO.Asset
   {
    Kind = AssetDb.DbEntry.EntryKind.Creature, // Future can do tiles and props
    Id = ExtraAssetsLibrary.DTO.Asset.GenerateID($"some sort of unique constant string"),
    GroupName = "Pokemon Gen I",
    Description = "Eevee, the evolution poke...",
    Name = "Eevee",
    groupTagOrder = 2, //Index in group
    Icon = icon, //Sprite (128x128)
    ModelCallback = ModelCallback, // Method
    
    // optional method to load custom base, null callback loads DefaultBase()
    BaseCallback = BaseCallback, 
};
ExtraAssetsLibrary.ExtraAssetPlugin.AddAsset(asset); // Name still WIP but methods the same.
```

Example Callback
```CSharp
public static GameObject ModelCallback(NGuid id){
  return new GameObject(); // You may want to return your actual model
}

public static GameObject BaseCallback(NGuid id){
  return new GameObject(); // You may want to return your actual custom base
}
```

## Recommended Extra Plugin (WIP)
In the works is an Asset Distribution plugin that will be paired with this plugin.

## Changelog
- 1.3.0: Planed Stable Release from 1.2.X,
- 1.2.11: Adjust presenter patcher to also accept placeables.
- 1.2.10: Config for failed spawns.
- 1.2.9: UI Prompt to clear failed spawns.
- 1.2.8: Fix icons packaging
- 1.2.7: Unstable Release
- 1.2.6: Fixed error being thrown in Harmony Patch due to Core TS Update.
- 1.2.5: Fixed Padding for new Icon Categories
- 1.2.4: Fixed blob errors being thrown
- 1.2.3: Repiped Effects via creature spawner
- 1.2.2: Added Asset ToolTip Dependency
- 1.2.1: Added FAP Dependencies and Icons
- 1.2.0: Added 2 new Catagories, (Aura and Effects) and (Slabs)
- 1.1.4: Global callback supplied for on asset load. Extra Asset information is now applicable for positions: [Torch, Spell Origin, Head Position (LOS), Hit Position (where spells aim for)]
- 1.1.3: pre-callbacks are moved to UI Event instead of pre-load allowing to stop spawn completely and only on try to spawn (e.g. trigger to adding an aura)
- 1.1.2: fix error thrown when tags are null
- 1.1.1: semi-fix base loading in. Refactor logging
- 1.1.0: Now allows callbacks before load and after placement.
- 1.0.4: Fixed zipping issue where re-entering campaign tries appending to db again
- 1.0.3: Assets can be added to existing groups
- 1.0.2: optimized package by pointing to existing default base instead of copy
- 1.0.1: prevent race condition.
- 1.0.0: Null callback stops creature spawn in prep for CMP Effects.
- 0.9.0: Alpha release

Shoutout to my Patreons on https://www.patreon.com/HolloFox recognising your
mighty contribution to my caffeine addiction:
- John Fuller
- [Tales Tavern](https://talestavern.com/) - MadWizard
