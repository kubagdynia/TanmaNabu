using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;
using TiledSharp;

namespace TanmaNabu.Core.Managers;

public class AssetManager
{
    private const string ResourcePath = "res";

    private static AssetManager _instance;
    private static readonly object Sync = new();

    #region SINGLETON

    private AssetManager() { }

    public static AssetManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
                
            lock (Sync)
            {
                if (_instance != null) return _instance;
                    
                var instance = new AssetManager();
                System.Threading.Thread.MemoryBarrier();
                _instance = instance;
            }
                
            return _instance;
        }
    }

    #endregion

    private IManager<Texture> _texture;

    public static IManager<Texture> Texture 
        => Instance._texture ?? (Instance._texture = new Manager<Texture>());

    private IManager<Font> _font;

    public static IManager<Font> Font 
        => Instance._font ?? (Instance._font = new Manager<Font>());

    private IManager<Music> _music;

    public static IManager<Music> Music 
        => Instance._music ?? (Instance._music = new Manager<Music>());

    private IManager<SoundBuffer> _sound;

    public static IManager<SoundBuffer> Sound
        => Instance._sound ?? (Instance._sound = new Manager<SoundBuffer>());

    private IManager<TmxMap> _map;

    public static IManager<TmxMap> Map
        => Instance._map ?? (Instance._map = new Manager<TmxMap>());

    private IManager<TmxTileset> _tileset;

    public static IManager<TmxTileset> Tileset
        => Instance._tileset ?? (Instance._tileset = new Manager<TmxTileset>());

    public string CombineResourcePathWith(string str1, string str2 = null, string str3 = null)
    {
        var pathSegments = new List<string>
        {
            Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]),
            ResourcePath,
            str1,
            str2,
            str3
        };
            
        return Path.Combine(pathSegments.Where(segment => !string.IsNullOrEmpty(segment)).ToArray()!);
    }

    public string GetFontPath(string fileName)
        => GetPath("fonts", fileName);

    public string GetMapPath(string fileName)
        => GetPath("maps", fileName);

    private string GetPath(string resourceType, string fileName)
        => Path.Combine(
            Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ?? string.Empty, ResourcePath, resourceType, fileName);

    public static void CleanUp()
    {
        Instance._texture?.RemoveAll();
        Instance._font?.RemoveAll();
        Instance._music?.RemoveAll();
        Instance._sound?.RemoveAll();
        Instance._map?.RemoveAll();
    }
}