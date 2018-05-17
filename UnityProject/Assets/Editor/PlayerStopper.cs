using UnityEditor;
using UnityEngine;
using XKTools;

/// <summary>
/// Stops current project execution when code changes are detected
/// </summary>
public class PlayerStopper : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        XKLog.EnableLogType("Editor", false);

        // detect scripts changes
        foreach (string str in importedAssets)
        {
            XKLog.Log("Editor", "Reimported Asset: " + str);
            if (str.EndsWith(".cs")
             || str.EndsWith(".dll"))
            {
                // stop updating player as scripts are fucked up
                if (EditorApplication.isPlaying)
                {
                    Debug.LogWarning("Further error have no incidence with the game, they are due to script recompilation");
                    EditorApplication.isPlaying = false;
                }
                break;
            }
        }


        /* might be useful for further treatments
        foreach (string str in deletedAssets)
            XKLog.Log("Editor", "Deleted Asset: " + str);
        foreach (string str in movedAssets)
            XKLog.Log("Editor", "Moved Asset: " + str);
        foreach (string str in movedFromAssetPaths)
            XKLog.Log("Editor", "Moved Asset From: " + str);
        */
    }
}