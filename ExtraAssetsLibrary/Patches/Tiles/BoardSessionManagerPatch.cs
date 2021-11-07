using System.Diagnostics;
using HarmonyLib;

namespace ExtraAssetsLibrary.Patches
{
    [HarmonyPatch(typeof(BoardSessionManager), "SetupClientAsDataModelClient")]
    public class BoardSessionManagerPatch
    {
        static void Prefix()
        {
            // UnityEngine.Debug.Log("Extra Asset Library Plugin:Board Session postfix");
            // var command = "talespire://asset/c80ffab5-b145-4d0a-a58a-00c66e7d2e42";
            // Process.Start(command).WaitForExit();
            
            // SingletonBehaviour<BoardToolManager>.Instance.SwitchToTool<BoardTool>();
        }
    }
}
