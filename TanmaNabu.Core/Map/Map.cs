using SFML.Graphics;
using SFML.System;
using System;

namespace TanmaNabu.Core.Map
{
    public class Map
    {
        private readonly float _switchTimeOfTileAnimations = 0.1f; // 100ms

        private float _totalTime;

        private ITileMap _backgroundTileMap;
        private ITileMap _foregroundTileMap;

        public MapData MapData { get; } = new MapData();

        public ITileMap GetBackgroundTileMap() => _backgroundTileMap;

        public ITileMap GetForegroundTileMap() => _foregroundTileMap;

        public void Load(string filename)
        {
            // Load map data to _mapData
            MapLoader.LoadMap(filename, MapData);

            LoadTileMaps();
        }

        public void Update(float deltaTime)
        {
            _totalTime += deltaTime;

            if (_totalTime >= _switchTimeOfTileAnimations)
            {
                _backgroundTileMap.Update(_totalTime);
                _foregroundTileMap.Update(_totalTime);

                // TODO: Switch background animation frame
                _totalTime = 0;
            }
        }

        public void SetWorldView(RenderTarget target, Vector2f center)
        {
            // TODO: this part should be optimized
            var view = new View
            {
                Size = new Vector2f(target.Size.X, target.Size.Y),
                Viewport = new FloatRect(0f, 0f, 1.0f, 1.0f),
            };
            view.Zoom(MapData.MapZoomFactor);

            var camCenterX = Math.Max(
                target.Size.X / 2.0f * MapData.MapZoomFactor,
                Math.Min(MapData.MapRec.Width * MapData.TileWorldDimension - target.Size.X / 2.0f * MapData.MapZoomFactor, center.X));

            var camCenterY = Math.Max(target.Size.Y / 2.0f * MapData.MapZoomFactor,
                Math.Min(MapData.MapRec.Height * MapData.TileWorldDimension - target.Size.Y / 2.0f * MapData.MapZoomFactor, center.Y));

            view.Center = new Vector2f(camCenterX, camCenterY);

            target.SetView(view);
        }

        private float Lerp(float value, float start, float end)
        {
            return start + (end - start) * value;
        }

        private void LoadTileMaps()
        {
            _backgroundTileMap = new TileMap();
            _backgroundTileMap.Load(MapData, MapData.BackgroundTileLayers);

            _foregroundTileMap = new TileMap();
            _foregroundTileMap.Load(MapData, MapData.ForegroundTileLayers);
        }
    }
}
