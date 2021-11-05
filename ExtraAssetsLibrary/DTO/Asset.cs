using System;
using System.Text;
using Bounce.Unmanaged;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace ExtraAssetsLibrary.DTO
{
    public enum CustomEntryKind
    {
        Tile,
        Creature,
        Prop,
        /// <summary>
        /// Planned to extend UI. Currently just setting up infrastructure.
        /// </summary>
        Aura,
    }

    public class Asset
    {
        public AssetDb.DbEntry.EntryKind Kind = AssetDb.DbEntry.EntryKind.Creature;

        /// <summary>
        /// Extends Kind to use CustomEntryKind instead of EntryKind
        /// </summary>
        public CustomEntryKind CustomKind
        {
            get => (CustomEntryKind) (int) Kind;
            set => Kind = (AssetDb.DbEntry.EntryKind) (int) value;
        }

        public NGuid Id;
        public string GroupName;
        public string Name;
        public string Description;
        public bool isDeprecated = false;
        public int groupTagOrder;
        public Sprite Icon;

        public float3 torchPos = new float3(0, 1.6f, 0);
        public float3 headPos = new float3(0,1,0);
        public float3 spellPos = new float3(0.1983986f, 0.7140511f, -0.3343165f);
        public float3 hitPos = new float3(-0.03812894f, 0.9740211f, -0.1837122f);

        // All Callbacks
        /// <summary>
        /// Occurs on clicking the AssetLoader GUI
        /// </summary>
        [CanBeNull] public Func<NGuid,bool> PreCallback;

        /// <summary>
        /// Occurs when base is being fetched and called
        /// </summary>
        [CanBeNull] public Func<NGuid, GameObject> BaseCallback;

        /// <summary>
        /// Occurs when model is being fetched and called
        /// </summary>
        public Func<NGuid, GameObject> ModelCallback;

        /// <summary>
        /// Occurs when model is placed.
        /// </summary>
        [CanBeNull] public Action<NGuid,CreatureGuid> PostCallback;

        public float3 Scale = 1;
        public float DefaultScale = 1;
        public float3 TransformOffset = Vector3.zero;
        public Quaternion Rotation = new Quaternion(0,0,0,0);
        public string[] tags;
        
        /// <summary>
        /// Generates a static NGuid based on a string.
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
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
