using System;
using System.Collections.Generic;
using ExtraAssetsLibrary.DTO;
using GameSequencer;

namespace ExtraAssetsLibrary.AssetDbExtension
{
    public static class ExtraDb
    {
        // new List<AssetDb.DbGroup>(128);
        internal static readonly DictionaryList<CustomEntryKind, List<AssetDb.DbGroup>> extraGroups =
            new DictionaryList<CustomEntryKind, List<AssetDb.DbGroup>>
            {
                {CustomEntryKind.Aura, new List<AssetDb.DbGroup>(128)}, // Aura and Effects
                {CustomEntryKind.Slab, new List<AssetDb.DbGroup>(128)},
            };
        internal static readonly Dictionary<string, List<AssetDb.DbEntry>> _auraByTag = new Dictionary<string, List<AssetDb.DbEntry>>(128);
        internal static readonly HashList<string> _auraTags = new HashList<string>();

        public static object call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        public static void SetValue(this object o, string methodName, object value)
        {
            var mi = o.GetType().GetField(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                mi.SetValue(o, value);
            }
        }

        public static Action<T1> GetMethod<T1>(this object o, string methodName)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            { 
                return (Action<T1>) Delegate.CreateDelegate(typeof(Action<T1>),mi);
            }
            return null;
        }
    }


}