using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtraAssetsLibrary.Patches
{
    class AssetDbTryGetTagsByKindPatch
    {
        public static bool PostFix(
            bool original,
            AssetDb.DbEntry.EntryKind kind,
            ref IReadOnlyList<string> tags
        )
        {
            List<string> temp = new List<string>();
            temp.AddRange(tags);
            var start = tags.Count;
            foreach (var asset in UI_AssetBrowserSetupAssetIndexPatch.assets.Where(a => a.Value.Kind == kind))
            {
                if (asset.Value.tags != null)
                    temp.AddRange(asset.Value.tags);
            }
            tags = temp.Distinct().ToList();
            var end = tags.Count;
            Debug.Log($"Added {end-start} tags");
            return original;
        }
    }
}
