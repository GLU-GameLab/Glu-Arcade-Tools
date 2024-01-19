using System.Collections.Generic;
using UnityEngine;

namespace ArcadeLauncher.Models
{
    [CreateAssetMenu(fileName = "game manifest", menuName ="Arcade/create manifest")]
    public class GameManifest : ScriptableObject
    {
        public string Name;
        public string Description;
        public List< string> Authors;
        public string NameExe;
        public string BackgroundColor;

        public int PlayersNeeded;
        [HideInInspector]public int ManifestVersion= 1;

        public string IconPath;
    }
}
