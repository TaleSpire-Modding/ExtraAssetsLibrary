using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;

namespace ExtraAssetsLibrary.Patches
{
    // [HarmonyPatch(typeof(AssetDb), "AddIdAsDummyPlaceable")]
    internal class AssetDbAddIdAsDummyPlaceablePatch
    {
        private static bool Prefix(NGuid id, PlaceableKind kind, ref BlobAssetReference<PlaceableData> __result
            , ref NativeList<BlobAssetReference<PlaceableData>> ____dummyPlaceables,
            ref NativeHashMap<NGuid, BlobView<PlaceableData>> ____placeables)
        {
            using (var blobBuilder = new BlobBuilder(Allocator.TempJob))
            {
                var builder = blobBuilder;
                ref var local = ref builder.ConstructRoot<PlaceableData>();
                local.Kind = kind;
                PlaceableData.ConstructDummyPlaceable(builder, ref local, id);
                var blobAssetReference = blobBuilder.CreateBlobAssetReference<PlaceableData>(Allocator.Persistent);
                ____dummyPlaceables.Add(in blobAssetReference);
                ____placeables.Add(id, blobAssetReference.TakeView());
                __result = blobAssetReference;
            }

            return false;
        }
    }
}