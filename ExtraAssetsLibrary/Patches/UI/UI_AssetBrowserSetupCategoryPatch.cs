using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtraAssetsLibrary.AssetDbExtension;
using ExtraAssetsLibrary.DTO;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI
{
    [HarmonyPatch(typeof(UI_AssetBrowser), "SetupCategory")]
    internal class UI_AssetBrowserSetupCategoryPatch
    {
        private static UI_AssetBrowser instance;

        internal static int setIndex;
        internal static UI_AssetBrowser searchListItem;
        internal static List<UIListItemClickEvents> _activeListItems;
        internal static AssetDb.DbGroup _searchFolder;

        public static void ListClickCallback(UIListItemClickEvents obj)
        {
            searchListItem.call("SelectWithoutNotification", obj == searchListItem);
            var folderIndex = 0;
            for (var index = 0; index < _activeListItems.Count; ++index)
                if (_activeListItems[index] != obj)
                {
                    _activeListItems[index].SelectWithoutNotification(false);
                }
                else
                {
                    _activeListItems[index].SelectWithoutNotification(true);
                    folderIndex = index;
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
                Debug.Log($"match, {setIndex}");
                if (setIndex == 3)
                {
                    var aura = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
                    var effects = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
                    var actual = ExtraDb.Zip(aura, effects);
                    var folder = actual[folderIndex];
                    instance.call("BrowseFolder", folder, false);
                    Parallel.ForEach(ExtraAssetPlugin.OnCatagoryChange, a => { a.Value(CustomEntryKind.Effects); });
                }

                if (setIndex > 4)
                {
                    var assets = UI_AssetBrowserSetupAssetIndexPatch._injecting[setIndex + 1];
                    var folder = assets[folderIndex];
                    instance.call("BrowseFolder", folder, false);
                    Parallel.ForEach(ExtraAssetPlugin.OnCatagoryChange,
                        a => { a.Value((CustomEntryKind) (setIndex + 1)); });
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
                for (var index1 = 0; index1 < _activeListItems.Count; ++index1)
                {
                    _activeListItems[index1].SelectWithoutNotification(false);
                    _activeListItems[index1].ForceReturnToFactory();
                }

                _activeListItems.Clear();
                Action<UIListItemClickEvents> ListClickCallback = UI_AssetBrowserSetupCategoryPatch.ListClickCallback;

                if (index == 3)
                {
                    var aura = UI_AssetBrowserSetupAssetIndexPatch._injecting[3];
                    var effects = UI_AssetBrowserSetupAssetIndexPatch._injecting[4];
                    var actual = ExtraDb.Zip(aura, effects);
                    for (var index1 = 0; index1 < actual.Count; ++index1)
                    {
                        var listItemClickEvents = _listItems.HireItem();
                        _activeListItems.Add(listItemClickEvents);
                        listItemClickEvents.SetupClick(index1, ListClickCallback);
                        listItemClickEvents.SetTitle(actual[index1].Name);
                    }
                }
                else if (index == 4)
                {
                    for (var index1 = 0; index1 < UI_AssetBrowserSetupAssetIndexPatch._injecting[5].Count; ++index1)
                    {
                        var listItemClickEvents = _listItems.HireItem();
                        _activeListItems.Add(listItemClickEvents);
                        listItemClickEvents.SetupClick(index1, ListClickCallback);
                        listItemClickEvents.SetTitle(UI_AssetBrowserSetupAssetIndexPatch._injecting[5][index1].Name);
                    }
                }
                else if (index == 5)
                {
                    for (var index1 = 0; index1 < UI_AssetBrowserSetupAssetIndexPatch._injecting[6].Count; ++index1)
                    {
                        var listItemClickEvents = _listItems.HireItem();
                        _activeListItems.Add(listItemClickEvents);
                        listItemClickEvents.SetupClick(index1, ListClickCallback);
                        listItemClickEvents.SetTitle(UI_AssetBrowserSetupAssetIndexPatch._injecting[6][index1].Name);
                    }
                }

                ___catagoryList.Arrange();
                if (____activeListItems.Count <= 0)
                    return false;
                ListClickCallback(_activeListItems.FirstOrDefault());
                __instance.call("MoveToAndSelectFolderLocation", 0, false);
                return false;
            }

            return true;
        }
    }
}