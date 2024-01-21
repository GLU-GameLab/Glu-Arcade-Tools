using System.Collections.Generic;
using UnityEngine;

namespace ArcadeLauncher.Models
{
    public class GameManifest
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
