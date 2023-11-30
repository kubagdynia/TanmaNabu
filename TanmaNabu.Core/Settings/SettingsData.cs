namespace TanmaNabu.Core.Settings
{
    public class SettingsData
    {
        public bool MusicEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        public byte MusicVolume { get; set; } = 100;    // 0..100
        public byte SoundVolume { get; set; } = 100;    // 0..100 
        public string MapsPath { get; set; } = "res/maps/";
        public string TilesetsPath { get; set; } = "res/tilesets/";
        public string TexturesPath { get; set; } = "res/textures/";
        public string FontsPath { get; set; } = "res/fonts/";
        public string BackgroundsPath { get; set; } = "res/backgrounds/";
        public string SoundsPath { get; set; } = "res/sounds/";
        public string MusicPath { get; set; } = "res/music/";
        public string ImagesPath { get; set; } = "res/images/";
        public string DllsPath { get; set; } = "res/dll/";
        public string TilesetFileExtension { get; set; } = ".tsx";
    }
}
