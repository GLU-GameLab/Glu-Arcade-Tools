using ArcadeLauncher.Models;
using System.IO;
using System.Text;
using UnityEngine;

public static class ManifestFile
{
    public static GameManifest current;

    private static string FileLocation;

    static ManifestFile()
    {
        FileLocation = Path.Combine(Application.dataPath, "../ProjectSettings", "GamelabManifest.json");

        if (File.Exists(FileLocation))
        {
            try
            {
                current = JsonUtility.FromJson<GameManifest>(File.ReadAllText(FileLocation));
            }
            catch
            {
                CreateNewManifest();

            }
        }
        else
        {
            CreateNewManifest();
        }
    }

    private static void CreateNewManifest()
    {
        current = new GameManifest
        {
            Name = Application.productName,
            PlayersNeeded = 1,
            ManifestVersion = 1,
            Description = "replace your description here",
            Authors = new string[] { System.Security.Principal.WindowsIdentity.GetCurrent().Name },
            NameExe = Application.productName + ".exe",
            BackgroundColor = ColorUtility.ToHtmlStringRGB(Color.green)
        };
    }

    public static void Save()
    {
        FileStream stream;
        if (!File.Exists(FileLocation))
            stream = File.Create(FileLocation);
        else
            stream = File.OpenWrite(FileLocation);


        byte[] info = new UTF8Encoding(true).GetBytes(JsonUtility.ToJson(current));
        stream.Write(info, 0, info.Length);
    }
}
