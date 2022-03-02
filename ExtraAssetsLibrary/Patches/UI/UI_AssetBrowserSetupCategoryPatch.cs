using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtraAssetsLibrary.AssetDbExtension;
using ExtraAssetsLibrary.DTO;
using HarmonyLib;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches.UI
{
    // [HarmonyPatch(typeof(UI_AssetBrowser), "SetupCategory")]
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
                var assets = UI_AssetBrowserSetupAssetIndexPatch._injecting[setIndex];
                var folder = assets[folderIndex];
                instance.call("BrowseFolder", folder, false);
                switch (setIndex)
                {
                    case 3:
                        Parallel.ForEach(ExtraAssetPlugin.OnCatagoryChange,
                            a => { a.Value(Category.AuraAndEffects); });
                        break;
                    case 4:
                        Parallel.ForEach(ExtraAssetPlugin.OnCatagoryChange,
                            a => { a.Value(Category.Slab); });
                        break;
                    case 5:
                        Parallel.ForEach(ExtraAssetPlugin.OnCatagoryChange,
                            a => { a.Value(Category.Audio); });
                        break;
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
            if (ExtraAssetPlugin.LogLevel.Value >= LogLevel.High) Debug.Log(__instance == null);

            var info = typeof(UI_AssetBrowser).GetField("_categoryList", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.Instance);
            var list = info.GetValue(__instance) as IList;
            foreach (var e in list)
            {
                List<AssetDb.DbGroup> groupList = (List<AssetDb.DbGroup>) e.GetType().GetField("groupList").GetValue(e);
                groupList.RemoveAll(g => ExtraAssetPlugin.HiddenGroups.Any(h => h.ToLower() == g.Name.ToLower()));
                e.GetType().GetField("groupList").SetValue(e,groupList);
            }
            info.SetValue(__instance,list);

            setIndex = index;
            instance = __instance;
            _searchFolder = ____searchFolder;
            if (6 > index && index > 2)
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

                var actual = UI_AssetBrowserSetupAssetIndexPatch._injecting[index];
                for (var index1 = 0; index1 < actual.Count; ++index1)
                {
                    var listItemClickEvents = _listItems.HireItem();
                    _activeListItems.Add(listItemClickEvents);
                    listItemClickEvents.SetupClick(index1, ListClickCallback);
                    listItemClickEvents.SetTitle(actual[index1].Name);
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