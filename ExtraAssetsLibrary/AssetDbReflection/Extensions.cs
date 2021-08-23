using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace PokeSpire.PokeSpire
{
    public static class Extensions
    {
        public static ParallelQuery<DirectoryInfo> GetSubfolders(this ParallelQuery<DirectoryInfo> query, string subfolder)
        {
            return query.Where(d => d.GetDirectories()
                    .Any(c => c.Name == subfolder))
                .Select(d => d.GetDirectories()
                    .Single(c => c.Name == subfolder)
                );
        }

        public static ParallelQuery<FileInfo> EnumerateFiles(this ParallelQuery<DirectoryInfo> query, string ext)
        {
            return query.SelectMany(c => c.EnumerateFiles().Where(d => d.FullName.EndsWith(ext)));
        }

        public static bool StrictKeyCheck(KeyboardShortcut check)
        {
            if (!check.IsUp()) return false;
            if (Input.GetKey(KeyCode.LeftAlt) != check.Modifiers.Contains(KeyCode.LeftAlt)) return false;
            if (Input.GetKey(KeyCode.RightAlt) != check.Modifiers.Contains(KeyCode.RightAlt)) return false;
            if (Input.GetKey(KeyCode.LeftControl) != check.Modifiers.Contains(KeyCode.LeftControl)) return false;
            if (Input.GetKey(KeyCode.RightControl) != check.Modifiers.Contains(KeyCode.RightControl)) return false;
            if (Input.GetKey(KeyCode.LeftShift) != check.Modifiers.Contains(KeyCode.LeftShift)) return false;
            if (Input.GetKey(KeyCode.RightShift) != check.Modifiers.Contains(KeyCode.RightShift)) return false;
            return true;
        }
    }
}
