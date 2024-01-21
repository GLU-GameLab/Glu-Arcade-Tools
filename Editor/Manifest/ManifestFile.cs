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

    public static void CreateNewManifest()
    {
        current = new GameManifest
        {
            Name = Application.productName,
            PlayersNeeded = 1,
            ManifestVersion = 1,
            Description = "replace your description here",
            Authors = new(),
            NameExe = Application.productName,
            BackgroundColor = ColorUtility.ToHtmlStringRGB(Color.green),
            IconPath = "Packages/com.gamelab.gamelab-arcade-tools/Editor/Default_icon.png",
        };
    }

    public static void Save()
    {
        FileStream stream;
        if (!File.Exists(FileLocation))
            File.Delete(FileLocation);

        stream = File.Create(FileLocation);

        byte[] info = new UTF8Encoding(true).GetBytes(JsonUtility.ToJson(current));
        stream.Write(info, 0, info.Length);
        stream.Close();
    }

    internal static void CopyTo(string manifestPath)
    {
        if (File.Exists(manifestPath))
            File.Delete(manifestPath);

        byte[] info = new UTF8Encoding(true).GetBytes(JsonUtility.ToJson(current));
        var stream = File.Create(manifestPath);
        stream.Write(info, 0, info.Length);
        stream.Close();
    }
}
