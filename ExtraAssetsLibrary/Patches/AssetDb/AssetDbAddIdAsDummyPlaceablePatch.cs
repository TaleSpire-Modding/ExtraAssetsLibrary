using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(AssetDb), "AddIdAsDummyPlaceable")]
    class AssetDbAddIdAsDummyPlaceablePatch
    {
        static bool Prefix(NGuid id, PlaceableKind kind, ref BlobAssetReference<PlaceableData> __result
        ,ref NativeList<BlobAssetReference<PlaceableData>> ____dummyPlaceables, ref NativeHashMap<NGuid, BlobView<PlaceableData>> ____placeables)
        {
            using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.TempJob))
            {
                BlobBuilder builder = blobBuilder;
                ref PlaceableData local = ref builder.ConstructRoot<PlaceableData>();
                local.Kind = kind;
                PlaceableData.ConstructDummyPlaceable(builder, ref local, id);
                BlobAssetReference<PlaceableData> blobAssetReference = blobBuilder.CreateBlobAssetReference<PlaceableData>(Allocator.Persistent);
                ____dummyPlaceables.Add(in blobAssetReference);
                ____placeables.Add(id, blobAssetReference.TakeView());
                __result = blobAssetReference;
            }
            return false;
        }
    }
}
