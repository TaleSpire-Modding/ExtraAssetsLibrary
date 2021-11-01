using System;
using System.Text;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.DTO
{
    public class Audio
    {
        public MusicData.MusicKind Kind = MusicData.MusicKind.Music;
        public NGuid Id;
        public string Name;
        public string Description;
    }
}
