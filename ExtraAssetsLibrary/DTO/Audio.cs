using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;

namespace ExtraAssetsLibrary.DTO
{
    public class Audio
    {
        public string Description;
        public NGuid Id;
        public MusicData.MusicKind Kind = MusicData.MusicKind.Music;
        public string Name;
    }
}