using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using System.IO;

public class ManifestBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder => throw new System.NotImplementedException();

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
        {
            Debug.LogWarning("For the game to work on the arcade, it must be a standalone windows build");
            return;
        }
        var sourceDirectory = Path.GetDirectoryName( pathToBuiltProject);
        var ManifestPath = Path.Combine(sourceDirectory, "Manifest.json");
        if (!File.Exists(ManifestPath))
            File.Create(ManifestPath).Close();

        File.WriteAllText(ManifestPath, pooep);
       
    }

    public void OnPreprocessBuild(BuildReport report)
    {
    }

    static string pooep = "{\r\n  \"Name\": \"Wrap it up\",\r\n  \"Description\": \"Wrap it Up is een Overcooked inspired spel waarin je zo snel mogelijk cadeautjes moet maken voor de kinderen. Er kan van alles fout gaan als je niet goed oplet, dus blijf scherp. Kunnen jullie de meeste cadeautjes maken?\",\r\n  \"Authors\": [\r\n    \"Justin van Diggelen\",\r\n    \"Didier Vermunt\",\r\n    \"Jipp Jansen\"\r\n  ],\r\n  \"NameExe\": \"Wrap-It-Up\",\r\n  \"NameFolder\": \"WrapItUp\",\r\n  \"BackgroundColor\": \"#f5d418\",\r\n  \"PlayersNeeded\": \"2\"\r\n}\r\n\r\n";
}
