using System;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ExtraAssetsLibrary.Handlers
{
    public static class BlobHandler
    {
        public static BlobString ConstructBlobString(string input)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            // builder.AllocateString(ref input, input.Length);
            return builder.CreateBlobAssetReference<BlobString>(Allocator.Persistent).Value;
        }

        public static BlobArray<BlobString> ConstructBlobData(string[] tags)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            try
            {
                ref var root = ref builder.ConstructRoot<BlobArray<BlobString>>();
                var nodearray = builder.Allocate(ref root, tags.Length);
                for (var i = 0; i < tags.Length; i++) nodearray[i] = ConstructBlobString(tags[i]);
                return builder.CreateBlobAssetReference<BlobArray<BlobString>>(Allocator.Persistent).Value;
            }
            catch (Exception e)
            {
                Debug.Log($"Extra Asset Library Plugin:ConstructBlobError:{e}");
            }

            return builder.CreateBlobAssetReference<BlobArray<BlobString>>(Allocator.Persistent).Value;
        }

        internal static BlobView<CreatureData> ToView(CreatureData cdata)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArray<CreatureData>>();
            var nodeArray = builder.Allocate(ref root, 1);
            nodeArray[0] = cdata;
            var blobArray = builder.CreateBlobAssetReference<BlobArray<CreatureData>>(Allocator.Persistent).Value;
            return blobArray.TakeView(0);
        }

        internal static BlobView<PlaceableData> ToView(PlaceableData cdata)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArray<PlaceableData>>();
            var nodeArray = builder.Allocate(ref root, 1);
            nodeArray[0] = cdata;
            var blobArray = builder.CreateBlobAssetReference<BlobArray<PlaceableData>>(Allocator.Persistent).Value;
            return blobArray.TakeView(0);
        }

        /*public static BlobPtr<AssetLoaderData.Packed> ConstructBlobPtr(AssetLoaderData.Packed pack)
        {
            BlobPtr<AssetLoaderData.Packed> o;

            return o;
        }*/
    }
}