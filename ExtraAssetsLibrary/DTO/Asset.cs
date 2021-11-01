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

        // All Callbacks
        [CanBeNull] public Func<NGuid,bool> PreCallback;
        [CanBeNull] public Func<NGuid, GameObject> BaseCallback;
        public Func<NGuid, GameObject> ModelCallback;
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
