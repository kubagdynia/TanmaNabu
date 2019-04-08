using System;
using System.IO;
using TanmaNabu.Core;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Settings.Exceptions;

namespace TanmaNabu.Settings
{
    class GameSettings
    {
        public static event EventHandler<SettingsPropertyType> GameSettingsChanged;

        private const string FileName = "settings.json";

        private static bool _settingsShouldBeSaved;

        private static SettingsData _settings = new SettingsData();

        public static bool MusicEnabled
        {
            get => _settings.MusicEnabled;
            set
            {
                if (_settings.MusicEnabled == value) return;

                _settings.MusicEnabled = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MusicEnabled);
                MarkToSaveSettings();
            }
        }

        public static bool SoundEnabled
        {
            get => _settings.SoundEnabled;
            set
            {
                if (_settings.SoundEnabled == value) return;

                _settings.SoundEnabled = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.SoundEnabled);
                MarkToSaveSettings();
            }
        }

        public static byte MusicVolume
        {
            get => _settings.MusicVolume;
            set
            {
                if (_settings.MusicVolume == value) return;

                _settings.MusicVolume = value > 100 ? (byte)100 : value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MusicVolume);
                MarkToSaveSettings();
            }
        }

        public static byte SoundVolume
        {
            get => _settings.SoundVolume;
            set
            {
                if (_settings.SoundVolume == value) return;

                _settings.SoundVolume = value > 100 ? (byte)100 : value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.SoundVolume);
                MarkToSaveSettings();
            }
        }

        public static string MapsPath
        {
            get => _settings.MapsPath;
            set
            {
                if (_settings.MapsPath == value) return;

                _settings.MapsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MapsPath);
                MarkToSaveSettings();
            }
        }

        public static string TilesetsPath
        {
            get => _settings.TilesetsPath;
            set
            {
                if (_settings.TilesetsPath == value) return;

                _settings.TilesetsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.TilesetsPath);
                MarkToSaveSettings();
            }
        }

        public static string TexturesPath
        {
            get => _settings.TexturesPath;
            set
            {
                if (_settings.TexturesPath == value) return;

                _settings.TexturesPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.TexturesPath);
                MarkToSaveSettings();
            }
        }

        public static string FontsPath
        {
            get => _settings.FontsPath;
            set
            {
                if (_settings.FontsPath == value) return;

                _settings.FontsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.FontsPath);
                MarkToSaveSettings();
            }
        }

        public static string BackgroundsPath
        {
            get => _settings.BackgroundsPath;
            set
            {
                if (_settings.BackgroundsPath == value) return;

                _settings.BackgroundsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.BackgroundsPath);
                MarkToSaveSettings();
            }
        }

        public static string SoundsPath
        {
            get => _settings.SoundsPath;
            set
            {
                if (_settings.SoundsPath == value) return;

                _settings.SoundsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.SoundsPath);
                MarkToSaveSettings();
            }
        }

        public static string MusicPath
        {
            get => _settings.MusicPath;
            set
            {
                if (_settings.MusicPath == value) return;

                _settings.MusicPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MusicPath);
                MarkToSaveSettings();
            }
        }

        public static string ImagesPath
        {
            get => _settings.ImagesPath;
            set
            {
                if (_settings.ImagesPath == value) return;

                _settings.ImagesPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.ImagesPath);
                MarkToSaveSettings();
            }
        }

        public static string TilesetFileExtension
        {
            get => _settings.TilesetFileExtension;
            set
            {
                if (_settings.TilesetFileExtension == value) return;

                _settings.TilesetFileExtension = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.TilesetFileExtension);
                MarkToSaveSettings();
            }
        }

        public static string GetExecutePath => Environment.CurrentDirectory;

        public static string GetFullPath(string itemPath, string value) => Path.Combine(GetExecutePath, itemPath, value);

        public static string GetFullPath(SettingsPropertyType propertyType, string value)
        {
            switch (propertyType)
            {
                case SettingsPropertyType.MapsPath:
                    return GetFullPath(MapsPath, value);
                case SettingsPropertyType.TilesetsPath:
                    return GetFullPath(TilesetsPath, value);
                case SettingsPropertyType.TexturesPath:
                    return GetFullPath(TexturesPath, value);
                case SettingsPropertyType.FontsPath:
                    return GetFullPath(FontsPath, value);
                case SettingsPropertyType.BackgroundsPath:
                    return GetFullPath(BackgroundsPath, value);
                case SettingsPropertyType.SoundsPath:
                    return GetFullPath(SoundsPath, value);
                case SettingsPropertyType.MusicPath:
                    return GetFullPath(MusicPath, value);
                case SettingsPropertyType.ImagesPath:
                    return GetFullPath(TilesetsPath, value);
                default:
                    throw new SettingsInvalidFullPathPropertyTypeException("Invalid Full Path Property Type value", propertyType.AllowedValues());
            }
        }

        public static void Save()
        {
            DataOperations.SaveData(_settings, FileName);
            _settingsShouldBeSaved = false;
        }

        public static void Load()
        {
            var settings = DataOperations.LoadData<SettingsData>(FileName, out bool fileExists);
            if (settings != null)
            {
                _settings = settings;
            }
            else if (!fileExists)
            {
#if DEBUG
                "Create settings file".Log();
#endif
                // If file is not exists then create file with default data
                Save();
            }

            _settingsShouldBeSaved = false;
        }

        public static void CleanUp()
        {
            if (_settingsShouldBeSaved)
            {
                Save();
            }
        }


        private static void MarkToSaveSettings()
        {
            _settingsShouldBeSaved = true;
        }

    }
}
