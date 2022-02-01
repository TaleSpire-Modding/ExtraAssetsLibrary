using System;
using UnityEngine.VFX;

namespace ExtraAssetsLibrary.DTO
{
    public class ProjectileProperties
    {
        public enum Path
        {
            Bezier,
            Linear,
            Lob
        }

        public void test()
        {
            
        }

        public Path PathType;

        /// <summary>
        /// Measured in Tiles Per Second
        /// </summary>
        public float Velocity;
        public Func<int> DragFunction;

    }
}
