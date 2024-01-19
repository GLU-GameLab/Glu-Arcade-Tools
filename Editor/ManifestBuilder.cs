using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public class ManifestBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
        {
            Debug.LogWarning("For the game to work on the arcade, it must be a standalone windows build");
            return;
        }
        var sourceDirectory = Path.GetDirectoryName(pathToBuiltProject);
        var ManifestPath = Path.Combine(sourceDirectory, "Manifest.json");


        ManifestFile.CopyTo(ManifestPath);

        var iconPath = Path.Combine(sourceDirectory, "icon.png");


        if (string.IsNullOrEmpty(ManifestFile.current.IconPath))
        {
            Debug.LogWarning("For the game to work on the arcade, a icon is required");
        }

        AssetDatabase.CopyAsset(ManifestFile.current.IconPath, iconPath);
        var iconMetaPath = iconPath + ".meta";
        if (File.Exists(iconMetaPath))
            File.Delete(iconMetaPath);
    }

    public void OnPreprocessBuild(BuildReport report)
    {
    }
}
