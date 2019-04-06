using SFML.System;

namespace SFML
{
    public static class SfmlSettings
    {
        public const string DllPath = "res/dll/";

        public const string CsfmlAudioPath = DllPath + CSFML.audio;
        public const string CsfmlGraphicsPath = DllPath + CSFML.graphics;
        public const string CsfmlSystemPath = DllPath + CSFML.system;
        public const string CsfmlWindowPath = DllPath + CSFML.window;
    }
}
