using System;
using System.Collections.Generic;
using System.Linq;
using ExtraAssetsLibrary.AssetDbExtension;
using ExtraAssetsLibrary.DTO;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "SetupCategory")]
    class UI_AssetBrowserSetupCategoryPatch
    {
        private static UI_AssetBrowser instance;

        internal static int setIndex;
        internal static UI_AssetBrowser searchListItem;
        internal static List<UIListItemClickEvents> _activeListItems;
        internal static AssetDb.DbGroup _searchFolder;

        public static void ListClickCallback(UIListItemClickEvents obj)
        {
            searchListItem.call("SelectWithoutNotification", obj == searchListItem);
            int folderIndex = 0;
            for (int index = 0; index < _activeListItems.Count; ++index)
            {
                if (_activeListItems[index] != obj)
                    _activeListItems[index].SelectWithoutNotification(false);
                else
                {
                    _activeListItems[index].SelectWithoutNotification(true);
                    folderIndex = index;
                }
            }

            if (!obj.Selected)
                return;
            if (obj == searchListItem)
            {
                if (_searchFolder == null)
                    return;
                instance.call("BrowseSearchFolder", _searchFolder, true);
            }
            else
            {
                if (setIndex == 3)
                {
                    Debug.Log("match, 3");
                    var assets = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
                    var folder = assets[folderIndex];
                    instance.call("BrowseFolder", folder,false);
                }
                else if (setIndex == 4)
                {
                    Debug.Log("match, 4");
                    var assets = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
                    var folder = assets[folderIndex];
                    instance.call("BrowseFolder", folder,false);
                }
            }
            
        }

        public static bool Prefix(ref int index,
            ref List<UIListItemClickEvents> ____activeListItems,
            ref SpawnFactory<UIListItemClickEvents> ____listItems,
            ref UI_List ___catagoryList,
            UI_AssetBrowser __instance,
            AssetDb.DbGroup ____searchFolder,
            ref UI_AssetBrowser ___searchListItem
            )
        {
            Debug.Log(__instance == null);
            setIndex = index;
            instance = __instance;
            _searchFolder = ____searchFolder;
            if (index > 2)
            {
                __instance.SetValue("_currentCategory", null);
                _activeListItems = ____activeListItems;
                searchListItem = ___searchListItem;
                var _listItems = ____listItems;
                for (int index1 = 0; index1 < _activeListItems.Count; ++index1)
                {
                    _activeListItems[index1].SelectWithoutNotification(false);
                    _activeListItems[index1].ForceReturnToFactory();
                }
                _activeListItems.Clear();
                Action<UIListItemClickEvents> ListClickCallback = UI_AssetBrowserSetupCategoryPatch.ListClickCallback;
                
                if (index == 3)
                {
                    for (int index1 = 0; index1 < ExtraDb.extraGroups[CustomEntryKind.Aura].Count; ++index1)
                    {
                        UIListItemClickEvents listItemClickEvents = _listItems.HireItem();
                        _activeListItems.Add(listItemClickEvents);
                        listItemClickEvents.SetupClick(index1, ListClickCallback);
                        listItemClickEvents.SetTitle(ExtraDb.extraGroups[CustomEntryKind.Aura][index1].Name);
                    }
                }
                else if (index == 4)
                {
                    for (int index1 = 0; index1 < ExtraDb.extraGroups[CustomEntryKind.Slab].Count; ++index1)
                    {
                        UIListItemClickEvents listItemClickEvents = _listItems.HireItem();
                        _activeListItems.Add(listItemClickEvents);
                        listItemClickEvents.SetupClick(index1, ListClickCallback);
                        listItemClickEvents.SetTitle(ExtraDb.extraGroups[CustomEntryKind.Slab][index1].Name);
                    }
                }

                ___catagoryList.Arrange();
                if (____activeListItems.Count <= 0)
                    return false;

                Debug.Log("Start call");
                ListClickCallback(_activeListItems.FirstOrDefault());
                
                __instance.call("MoveToAndSelectFolderLocation", 0, false);
                Debug.Log("End call");
                return false;
            }
            return true;
        }
    }
}
