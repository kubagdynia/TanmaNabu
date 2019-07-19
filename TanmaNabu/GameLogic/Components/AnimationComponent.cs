using Entitas;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TanmaNabu.Core.Animation;
using TanmaNabu.Core.Managers;
using TanmaNabu.Core.Settings;
using TiledSharp;

namespace TanmaNabu.GameLogic.Components
{
    public sealed class AnimationComponent : IComponent
    {
        private float switchTime = 0.15f;

        private float totalTime = 0;

        private List<AnimationFrame> _frames;

        private List<AnimationFrame> Frames => _frames ?? (_frames = new List<AnimationFrame>());

        private Sprite _sprite { get; set; }

        private string _tilesetName;

        private string _objectType;

        private int _spriteWorldDimension;

        private AnimationType _currentAnimationType { get; set; } = AnimationType.Idle;

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
            totalTime += deltaTime;

            if (totalTime >= switchTime)
            {
                totalTime = 0;

                // Get next animation frame
                if (CurrentAnimationFrame == null)
                {
                    CurrentAnimationFrame = Frames.FirstOrDefault(x => x.AnimationType == _currentAnimationType);
                }
                else
                {
                    CurrentAnimationFrame = Frames.FirstOrDefault(x => x.AnimationType == _currentAnimationType && x.Id != CurrentAnimationFrame.Id);
                }

                if (CurrentAnimationFrame == null)
                {
                    CurrentAnimationFrame = Frames.Skip(1).FirstOrDefault(x => x.AnimationType == AnimationType.WalkDown);
                    switchTime = float.MaxValue;
                }
                else
                {
                    switchTime = (float)CurrentAnimationFrame.Duration / 1000;
                }

                SetSprite();
            }
        }

        public void UpdateSpritePosition(float x, float y)
        {
            if (_sprite == null)
            {
                return;
            }

            _sprite.Position = new Vector2f(x, y);
        }

        public void UpdateAnimationType(AnimationType animationType)
        {
            if (_currentAnimationType == animationType)
            {
                return;
            }

            _currentAnimationType = animationType;

            UpdateAnimation(switchTime);
        }

        public Sprite GetSprite() => _sprite;

        public FloatRect GetSpriteGlobalBounds() => _sprite.GetGlobalBounds();

        public int GetCurrentTiledId() => CurrentAnimationFrame.Id;

        private bool LoadTexture()
        {
            if (string.IsNullOrEmpty(_tilesetName))
            {
                return false;
            }

            TmxTileset tileset = AssetManager.Instance.Tileset.Get(_tilesetName);

            if (tileset == null || tileset.Image == null)
            {
                return false;
            }

            string filename = Path.GetFileName(tileset.Image.Source);
            string texturePath = GameSettings.GetFullPath(SettingsPropertyType.TexturesPath, filename);

            AssetManager.Instance.Texture.Load(filename, GameSettings.GetFullPath(
                    SettingsPropertyType.TexturesPath, filename));            

            Texture = AssetManager.Instance.Texture.LoadAndGet(filename, texturePath);

            return true;
        }

        private bool InitializeFrames(string objectType)
        {
            if (string.IsNullOrEmpty(_tilesetName))
            {
                return false;
            }

            TmxTileset tileset = AssetManager.Instance.Tileset.Get(_tilesetName);

            if (tileset == null || tileset.Image == null || tileset.Tiles == null)
            {
                return false;
            }

            // If the objectType is not null than take only the tiles that correspond to that objectType
            List<TmxTilesetTile> tiles;
            if (objectType == null)
            {
                tiles = tileset.Tiles.Where(c => c.AnimationFrames != null && c.AnimationFrames.Any()).ToList();
            }
            else
            {
                tiles = tileset.Tiles.Where(c => c.AnimationFrames != null && c.Type != null && c.AnimationFrames.Any() &&
                    c.Type.Equals(objectType, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            foreach (var tile in tiles)
            {
                if (tile.Properties == null)
                {
                    continue;
                }

                if (!Enum.TryParse(
                    tile.Properties.FirstOrDefault(c => c.Key.Equals("AnimationType", StringComparison.InvariantCultureIgnoreCase)).Value,
                    out AnimationType animationType))
                {
                    continue;
                }

                foreach (var animationFrame in tile.AnimationFrames)
                {
                    int xIndex = animationFrame.Id % tileset.Columns.Value;
                    int yIndex = animationFrame.Id / tileset.Columns.Value;

                    AnimationFrame frame = new AnimationFrame(
                        animationFrame.Id,
                        animationFrame.Duration,
                        animationType,
                        new IntRect(
                            xIndex * tileset.TileWidth,
                            yIndex * tileset.TileHeight,
                            tileset.TileWidth,
                            tileset.TileHeight));

                    Frames.Add(frame);
                }
            }

            SetDefaultIdleFrame();

            return true;
        }

        private void SetDefaultIdleFrame()
        {
            // Set default idle frame
            AnimationFrame idleFrame = Frames.FirstOrDefault(x => x.AnimationType == AnimationType.Idle);

            // If idle frame is not set try to set first down walk frame
            if (idleFrame == null)
            {
                idleFrame = Frames.FirstOrDefault(x => x.AnimationType == AnimationType.WalkDown);
            }

            switchTime = (float)idleFrame.Duration / 1000;

            CurrentAnimationFrame = idleFrame;

            SetSprite();
        }

        private void SetSprite()
        {
            if (_sprite == null)
            {
                _sprite = new Sprite(Texture)
                {
                    Origin = new Vector2f(CurrentAnimationFrame.Rect.Width / 2, CurrentAnimationFrame.Rect.Height / 2), // 8, 8
                    Scale = new Vector2f(_spriteWorldDimension, _spriteWorldDimension)
                };
            }

            if (_sprite.TextureRect != CurrentAnimationFrame.Rect)
            {
                _sprite.TextureRect = CurrentAnimationFrame.Rect;
            }
        }
    }
}
