namespace ArcadeLauncher.Models
{
    public class GameManifest
    {
        public string Name;
        public string Description;
        public string[] Authors;
        public string NameExe;
        public string BackgroundColor;

        public int PlayersNeeded;
        public int ManifestVersion= 1;
    }
}
