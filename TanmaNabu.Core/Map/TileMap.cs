using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TiledSharp;

namespace TanmaNabu.Core.Map
{
    public class TileMap : ITileMap
    {
        private const byte TileVertices = 4; // each tile has 4 vertices

        private Texture _tileset;
        private MapData _mapData;

        private readonly Collection<MapLayer> _mapLayers = new Collection<MapLayer>();

        private Vector2f _size;
        private Vector2f _translation;
        private Vector2i _downRight;
        private Vector2i _topLeft;

        public void Draw(RenderTarget target, RenderStates states)
        {
            // Draw only visible tiles
            var view = target.GetView();

            _size = GetViewSize(view);

            _translation = GetViewCenter(view) - _size / 2.0f;

            _downRight = GetViewDownRight(_size, _translation);
            _topLeft = GetViewTopLeft(_translation);

            var tilesCount = (uint)((_downRight.X + 1) * TileVertices);

            states.Texture = _tileset;

            foreach (var mapLayer in _mapLayers)
            {
                for (var y = _topLeft.Y; y <= _downRight.Y; y++)
                {
                    DrawVertices(target, states, tilesCount, mapLayer.VerticesLayer, y);
                }
            }
        }

        public void Load(MapData data, IList<TmxLayer> layers)
        {
            if (layers == null || !layers.Any())
            {
                return;
            }

            _mapData = data;

            var tileset = _mapData.Tilesets.FirstOrDefault();

            _tileset = new Texture(tileset.ImagePath);

            var tilesetColumns = (int)_tileset.Size.X / data.TileSize.X;

            foreach (var layer in layers)
            {
                var mapLayer = new MapLayer();
                var vertices = new Vertex[(uint)(_mapData.MapSize.X * _mapData.MapSize.Y * TileVertices)];

                var verticeIndex = 0;
                foreach (TmxLayerTile tileItem in layer.Tiles)
                {
                    AddTileVertices(vertices, verticeIndex, tileItem, tilesetColumns);

                    verticeIndex += TileVertices; // increase by 4 because each tile has 4 vertices
                }

                mapLayer.VerticesLayer = vertices;

                _mapLayers.Add(mapLayer);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var mapLayer in _mapLayers)
            {

            }
        }

        private void DrawVertices(RenderTarget target, RenderStates states, uint tilesCount, Vertex[] vertices, int lineNumber)
        {
            var start = (uint)(lineNumber * _mapData.MapSize.X * TileVertices);

            if (start >= vertices.Length)
            {
                return;
            }

            if (start + tilesCount > vertices.Length)
            {
                tilesCount = (uint)(vertices.Length - start);
            }

            target.Draw(vertices, start, tilesCount, PrimitiveType.Quads, states);
        }

        private void AddTileVertices(Vertex[] vertices, int verticeIndex, TmxLayerTile tileItem, int tilesetColumns)
        {
            var a1 = tileItem.VerticalFlip;
            var a2 = tileItem.HorizontalFlip;
            var a3 = tileItem.DiagonalFlip;

            var xIndex = (tileItem.Gid - 1) % tilesetColumns;
            var yIndex = (tileItem.Gid - 1) / tilesetColumns;

            AddTileVertices(vertices, verticeIndex, xIndex, yIndex, new Vector2f(tileItem.X, tileItem.Y),
                tileItem.VerticalFlip, tileItem.HorizontalFlip, tileItem.DiagonalFlip);
        }

        private unsafe void AddTileVertices(Vertex[] vertices, int verticeIndex, int x, int y, Vector2f position,
            bool verticalFlip = false, bool horizontalFlip = false, bool diagonalFlip = false)
        {
            var tileWorldDimension = GetWorldTileSize.X * _mapData.TileWorldDimension;

            fixed (Vertex* fptr = vertices)
            {
                var ptr = fptr + verticeIndex;

                Vector2f[] textCoords = new Vector2f[]
                {
                    new Vector2f(GetWorldTileSize.X * x, GetWorldTileSize.Y * y),
                    new Vector2f(GetWorldTileSize.X * x + GetWorldTileSize.X, GetWorldTileSize.Y * y),
                    new Vector2f(GetWorldTileSize.X * x + GetWorldTileSize.X, GetWorldTileSize.Y * y + GetWorldTileSize.Y),
                    new Vector2f(GetWorldTileSize.X * x, GetWorldTileSize.X * y + GetWorldTileSize.Y)
                };

                if (horizontalFlip)
                {
                    textCoords = new Vector2f[]
                    {
                        textCoords[1],
                        textCoords[0],
                        textCoords[3],
                        textCoords[2]
                    };
                }

                if (verticalFlip)
                {
                    textCoords = new Vector2f[]
                    {
                        textCoords[3],
                        textCoords[2],
                        textCoords[1],
                        textCoords[0]
                    };
                }

                if (diagonalFlip)
                {
                    textCoords = new Vector2f[]
                    {
                        textCoords[1],
                        textCoords[2],
                        textCoords[3],
                        textCoords[0]
                    };
                }

                //if (diagonalFlip)
                //{
                //    textCoords = new Vector2f[]
                //    {
                //        textCoords[3],
                //        textCoords[0],
                //        textCoords[1],
                //        textCoords[2]
                //    };
                //}



                ptr->Position = position * tileWorldDimension;
                ptr->TexCoords = textCoords[0];
                ptr->Color = Color.White;

                ptr++;

                ptr->Position = (new Vector2f(1.0f, 0.0f) + position) * tileWorldDimension;
                ptr->TexCoords = textCoords[1];
                ptr->Color = Color.White;
                ptr++;

                ptr->Position = (new Vector2f(1.0f, 1.0f) + position) * tileWorldDimension;
                ptr->TexCoords = textCoords[2];
                ptr->Color = Color.White;
                ptr++;

                ptr->Position = (new Vector2f(0.0f, 1.0f) + position) * tileWorldDimension;
                ptr->TexCoords = textCoords[3];
                ptr->Color = Color.White;
            }
        }

        private Vector2i GetViewDownRight(Vector2f size, Vector2f translation)
        {
            return new Vector2i((int)(translation + size).X / GetWorldTileSize.X, (int)(translation + size).Y / GetWorldTileSize.Y);
        }

        private Vector2i GetViewTopLeft(Vector2f translation)
        {
            return new Vector2i((int)translation.X / GetWorldTileSize.X, (int)translation.Y / GetWorldTileSize.Y);
        }

        private Vector2f GetViewSize(View view)
        {
            return view.Size / _mapData.TileWorldDimension;
        }

        private Vector2f GetViewCenter(View view)
        {
            return view.Center / _mapData.TileWorldDimension;
        }

        private Vector2i GetWorldTileSize => _mapData.TileSize;
    }
}
