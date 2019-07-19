using System;
using System.IO;
using SFML.Audio;
using SFML.Graphics;
using TiledSharp;

namespace TanmaNabu.Core.Managers
{
    public class AssetManager
    {
        private const string ResourcePath = "res";

        private static AssetManager _instance;
        private static readonly object Sync = new object();

        #region SINGLETON

        private AssetManager() { }

        public static AssetManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                        {
                            AssetManager instance = new AssetManager();
                            System.Threading.Thread.MemoryBarrier();
                            _instance = instance;
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        private IManager<Texture> _texture;

        public IManager<Texture> Texture => _texture ?? (_texture = new Manager<Texture>());

        private IManager<Font> _font;

        public IManager<Font> Font => _font ?? (_font = new Manager<Font>());

        private IManager<Music> _music;

        public IManager<Music> Music => _music ?? (_music = new Manager<Music>());

        private IManager<SoundBuffer> _sound;

        public IManager<SoundBuffer> Sound => _sound ?? (_sound = new Manager<SoundBuffer>());

        private IManager<TmxMap> _map;

        public IManager<TmxMap> Map => _map ?? (_map = new Manager<TmxMap>());

        private IManager<TmxTileset> _tileset;

        public IManager<TmxTileset> Tileset => _tileset ?? (_tileset = new Manager<TmxTileset>());

        public string CombineResourcePathWith(string str1, string str2 = "", string str3 = "")
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), ResourcePath, str1, str2, str3);

            return path;
        }

        public string GetFontPath(string fileName) => GetPath("fonts", fileName);

        public string GetMapPath(string fileName) => GetPath("maps", fileName);

        private string GetPath(string resourceType, string fileName)
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), ResourcePath, resourceType, fileName);

            return path;
        }

    }
}
