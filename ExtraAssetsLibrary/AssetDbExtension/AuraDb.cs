﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtraAssetsLibrary.AssetDbExtension
{
    public static class ExtraDb
    {
        public static List<AssetDb.DbGroup> Zip(List<AssetDb.DbGroup> list1, List<AssetDb.DbGroup> list2)
        {
            var actual = new List<AssetDb.DbGroup>();
            actual.AddRange(list1);
            foreach (var item in list2)
                if (actual.Any(a => a.Name == item.Name))
                {
                    var group = actual.Single(a => a.Name == item.Name);
                    @group.Entries.AddRange(item.Entries);
                    @group.Entries.OrderBy(a => a.Name).ToList();
                }
                else
                {
                    actual.Add(item);
                }

            actual = actual.OrderBy(a => a.Name).ToList();
            return actual;
        }

        public static object call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null) return mi.Invoke(o, args);
            return null;
        }

        public static void call<T>(string methodName, params object[] args)
        {
            var myClassType = Assembly.GetExecutingAssembly().GetType(typeof(T).Namespace + ".myClass"); 
            myClassType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, args);
        }

        public static I GetValue<T,I>(string methodName)
        {
            var mi = typeof(T).GetField(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            return (I) mi?.GetValue(null);
        }


        public static void SetValue(this object o, string methodName, object value)
        {
            var mi = o.GetType().GetField(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null) mi.SetValue(o, value);
        }

        public static Action<T1> GetMethod<T1>(this object o, string methodName)
        {
            var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null) return (Action<T1>) Delegate.CreateDelegate(typeof(Action<T1>), mi);
            return null;
        }
    }
}