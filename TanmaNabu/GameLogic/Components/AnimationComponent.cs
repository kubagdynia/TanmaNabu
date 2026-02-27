using Entitas;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TanmaNabu.Core.Animation;
using TanmaNabu.Core.Managers;
using TanmaNabu.Core.Settings;
using TiledSharp;

namespace TanmaNabu.GameLogic.Components;

public sealed class AnimationComponent : IComponent
{
    private float _switchTime = 0.15f;
    private float _totalTime  = 0;

    // Index of the current frame within the active animation sequence
    private int _currentFrameIndex = -1;

    // Cached frame list for the current animation type
    private List<AnimationFrame> _currentTypeFrames;

    // All frames grouped by type – built once on initialisation, never reallocated
    private readonly Dictionary<AnimationType, List<AnimationFrame>> _framesByType = new();

    private List<AnimationFrame> _frames;

    private List<AnimationFrame> Frames => _frames ??= [];

    private Sprite Sprite { get; set; }

    private string _tilesetName;

    private string _objectType;

    private int _spriteWorldDimension;

    private AnimationType CurrentAnimationType { get; set; } = AnimationType.Idle;

    public Texture Texture { get; set; }

    public Time FrameTime { get; set; }

    public AnimationFrame CurrentAnimationFrame { get; set; }

    public bool Looped { get; set; }

    public AnimationComponent()
    {

    }

    public void SetTileset(string tilesetName, string objectType, int spriteWorldDimension)
    {
        _tilesetName = tilesetName;
        _objectType = objectType;
        _spriteWorldDimension = spriteWorldDimension;
        LoadTexture();
        InitializeFrames(objectType);
    }

    public void UpdateAnimation(float deltaTime)
    {
        _totalTime += deltaTime;

        while (_totalTime >= _switchTime)
        {
            // Carry over leftover time so frame timing stays accurate
            _totalTime -= _switchTime;

            var frames = _currentTypeFrames;

            if (frames == null || frames.Count == 0)
            {
                // Fallback: no frames for this type
                CurrentAnimationFrame = Frames.Skip(1).FirstOrDefault(x => x.AnimationType == AnimationType.WalkDown);
                _switchTime = float.MaxValue;
                SetSprite();
                return;
            }

            // _currentFrameIndex starts at -1 after a type change, so the first +1 lands on 0
            _currentFrameIndex = (_currentFrameIndex + 1) % frames.Count;
            CurrentAnimationFrame = frames[_currentFrameIndex];
            _switchTime = (float)CurrentAnimationFrame.Duration / 1000;

            SetSprite();
        }
    }

    public void UpdateSpritePosition(float x, float y)
    {
        if (Sprite == null)
        {
            return;
        }

        Sprite.Position = new Vector2f(x, y);
    }

    public void UpdateAnimationType(AnimationType animationType)
    {
        if (CurrentAnimationType == animationType)
            return;

        CurrentAnimationType = animationType;

        // O(1) lookup – no allocation
        _framesByType.TryGetValue(CurrentAnimationType, out _currentTypeFrames);

        _currentFrameIndex = -1;
        _totalTime = 0;

        if (_currentTypeFrames is { Count: > 0 })
        {
            CurrentAnimationFrame = _currentTypeFrames[0];
            _switchTime = (float)CurrentAnimationFrame.Duration / 1000;
            SetSprite();
        }
    }

    public Sprite GetSprite() => Sprite;

    public FloatRect GetSpriteGlobalBounds() => Sprite.GetGlobalBounds();

    public int GetCurrentTiledId() => CurrentAnimationFrame.Id;

    private bool LoadTexture()
    {
        if (string.IsNullOrEmpty(_tilesetName))
        {
            return false;
        }

        var tileset = AssetManager.Tileset.Get(_tilesetName);

        if (tileset?.Image == null)
        {
            return false;
        }

        var filename = Path.GetFileName(tileset.Image.Source);
        var texturePath = GameSettings.GetFullPath(SettingsPropertyType.TexturesPath, filename);

        AssetManager.Texture.Load(filename, GameSettings.GetFullPath(
            SettingsPropertyType.TexturesPath, filename));            

        Texture = AssetManager.Texture.LoadAndGet(filename, texturePath);

        return true;
    }

    private bool InitializeFrames(string objectType)
    {
        if (string.IsNullOrEmpty(_tilesetName))
        {
            return false;
        }

        var tileset = AssetManager.Tileset.Get(_tilesetName);

        if (tileset?.Image == null || tileset.Tiles == null)
        {
            return false;
        }

        // If the objectType is not null than take only the tiles that correspond to that objectType
        List<TmxTilesetTile> tiles;
        if (objectType == null)
        {
            tiles = tileset.Tiles.Where(c => c.AnimationFrames != null && c.AnimationFrames.Count != 0).ToList();
        }
        else
        {
            tiles = tileset.Tiles.Where(c => c.AnimationFrames != null && c.Type != null && c.AnimationFrames.Count != 0 &&
                                             c.Type.Equals(objectType, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        foreach (var tile in tiles.Where(tile => tile.Properties != null))
        {
            if (!Enum.TryParse(
                    tile.Properties.FirstOrDefault(c => c.Key.Equals("AnimationType", StringComparison.InvariantCultureIgnoreCase)).Value,
                    out AnimationType animationType))
            {
                continue;
            }

            foreach (var animationFrame in tile.AnimationFrames)
            {
                var xIndex = animationFrame.Id % tileset.Columns.Value;
                var yIndex = animationFrame.Id / tileset.Columns.Value;

                var frame = new AnimationFrame(
                    animationFrame.Id,
                    animationFrame.Duration,
                    animationType,
                    new IntRect(
                        new Vector2i(xIndex * tileset.TileWidth, yIndex * tileset.TileHeight),
                        new Vector2i(tileset.TileWidth, tileset.TileHeight)));

                Frames.Add(frame);
            }
        }

        SetDefaultIdleFrame();

        // Group all frames by animation type once – O(n) at load time, O(1) at runtime
        _framesByType.Clear();
        foreach (var frame in Frames)
        {
            if (!_framesByType.TryGetValue(frame.AnimationType, out var list))
            {
                list = new List<AnimationFrame>();
                _framesByType[frame.AnimationType] = list;
            }
            list.Add(frame);
        }

        // Initialise the active frame list from the dictionary
        _framesByType.TryGetValue(CurrentAnimationType, out _currentTypeFrames);

        return true;
    }

    private void SetDefaultIdleFrame()
    {
        var idleFrame = Frames.FirstOrDefault(x => x.AnimationType == AnimationType.Idle);

        if (idleFrame == null)
        {
            idleFrame = Frames.FirstOrDefault(x => x.AnimationType == AnimationType.WalkDown);
            if (idleFrame != null) CurrentAnimationType = AnimationType.WalkDown;
        }

        if (idleFrame != null)
        {
            _switchTime = (float)idleFrame.Duration / 1000;
            CurrentAnimationFrame = idleFrame;
            _currentFrameIndex = -1;
        }

        SetSprite();
    }

    private void SetSprite()
    {
        Sprite ??= new Sprite(Texture)
        {
            Origin = new Vector2f(CurrentAnimationFrame.Rect.Width / 2, CurrentAnimationFrame.Rect.Height / 2), // 8, 8
            Scale = new Vector2f(_spriteWorldDimension, _spriteWorldDimension)
        };

        if (Sprite.TextureRect != CurrentAnimationFrame.Rect)
        {
            Sprite.TextureRect = CurrentAnimationFrame.Rect;
        }
    }
}