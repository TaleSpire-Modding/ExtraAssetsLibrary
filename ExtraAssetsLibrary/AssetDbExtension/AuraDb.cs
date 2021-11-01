using System.Collections.Generic;

namespace ExtraAssetsLibrary.AssetDbExtension
{
    public static class AuraDb
    {
        internal static readonly List<AssetDb.DbGroup> _auraGroups = new List<AssetDb.DbGroup>(128);
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
    }


}