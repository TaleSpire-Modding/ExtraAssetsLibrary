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
- 1.0.3: Assets can be added to existing groups
- 1.0.2: optimized package by pointing to existing default base instead of copy
- 1.0.1: prevent race condition.
- 1.0.0: Null callback stops creature spawn in prep for CMP Effects.
- 0.9.0: Alpha release

Shoutout to my Patreons on https://www.patreon.com/HolloFox recognising your
mighty contribution to my caffeine addiction:
- John Fuller
- [Tales Tavern](https://talestavern.com/) - MadWizard