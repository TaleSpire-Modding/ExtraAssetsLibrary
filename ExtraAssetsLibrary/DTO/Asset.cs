using System;
using System.Text;
using Bounce.Unmanaged;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using MD5 = System.Security.Cryptography.MD5;

namespace ExtraAssetsLibrary.DTO
{
    public enum CustomEntryKind
    {
        Tile,
        Creature,
        Prop,

        /// <summary>
        ///     Planned to extend UI. Currently just setting up infrastructure.
        /// </summary>
        Aura,
        Effects,
        Slab,
        Audio,
        Projectile,
    }

    public enum Category
    {
        Tile,
        Creature,
        Prop,
        AuraAndEffects,
        Slab,
        Audio,
    }

    public class Asset
    {
        /// <summary>
        ///     Occurs when base is being fetched and called
        /// </summary>
        [CanBeNull] public Func<NGuid, GameObject> BaseCallback;
        
        public float DefaultScale = 1;
        public string Description;
        public string GroupName;
        public int groupTagOrder;
        public float3 headPos = new float3(0, 1, 0);
        public float3 hitPos = new float3(-0.03812894f, 0.9740211f, -0.1837122f);
        public Sprite Icon;
        public Category Category = (DTO.Category) 7;

        public NGuid Id;
        public bool isDeprecated = false;
        public AssetDb.DbEntry.EntryKind Kind = AssetDb.DbEntry.EntryKind.Creature;

        /// <summary>
        ///     Occurs when model is being fetched and called
        /// </summary>
        public Func<NGuid, GameObject> ModelCallback;

        public string Name;

        /// <summary>
        ///     Occurs when model is placed.
        /// </summary>
        [CanBeNull] public Action<NGuid, CreatureGuid> PostCallback;

        // All Callbacks
        /// <summary>
        ///     Occurs on clicking the AssetLoader GUI
        /// </summary>
        [CanBeNull] public Func<NGuid, bool> PreCallback;

        public Quaternion Rotation = new Quaternion(0, 0, 0, 0);

        public float3 Scale = 1;
        public float3 spellPos = new float3(0.1983986f, 0.7140511f, -0.3343165f);
        public string[] tags;

        public TileProperties TileProperties = new TileProperties();

        public float3 torchPos = new float3(0, 1.6f, 0);
        public float3 TransformOffset = Vector3.zero;

        /// <summary>
        ///     Extends Kind to use CustomEntryKind instead of EntryKind
        /// </summary>
        public CustomEntryKind CustomKind
        {
            get => (CustomEntryKind) (int) Kind;
            set => Kind = (AssetDb.DbEntry.EntryKind) (int) value;
        }

        /// <summary>
        ///     Generates a static NGuid based on a string.
        /// </summary>
        /// <param name="id">Text used to generate NGuid, must be consistent</param>
        /// <returns></returns>
        public static NGuid GenerateID(string id)
        {
            return new NGuid(Guid.Parse(CreateMD5(id)));
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString();
            }
        }
    }
}