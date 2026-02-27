using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TiledSharp;

namespace TanmaNabu.Core.Map;

public class TileMap : ITileMap
{
    // 6 vertices per tile: two triangles (top-left, top-right, bottom-right) + (top-left, bottom-right, bottom-left)
    private const int TileVertices = 6;

    private Texture _tileset;
    private MapData _mapData;

    private readonly Collection<MapLayer> _mapLayers = new();

    // Cached per-frame view values – avoids repeated property access in Draw
    private Vector2f _size;
    private Vector2f _translation;
    private Vector2i _downRight;
    private Vector2i _topLeft;

    public void Draw(RenderTexture target, RenderStates states)
    {
        var view = target.GetView();

        _size        = GetViewSize(view);
        _translation = GetViewCenter(view) - _size / 2.0f;
        _downRight   = GetViewDownRight(_size, _translation);
        _topLeft     = GetViewTopLeft(_translation);

        // Vertices from tile 0 to _downRight.X – must start from 0 because the
        // vertex array is laid out row by row starting at tile X=0.
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

    public void Load(MapData data, System.Collections.Generic.IList<TmxLayer> layers)
    {
        if (layers == null || !layers.Any())
            return;

        _mapData = data;

        var tileset = _mapData.Tilesets.FirstOrDefault();
        _tileset = new Texture(tileset.ImagePath);
        // Enable texture smoothing off and set repeated for tiling
        _tileset.Smooth   = false;
        _tileset.Repeated = true;

        var tilesetColumns = (int)_tileset.Size.X / data.TileSize.X;

        foreach (var layer in layers)
        {
            var mapLayer = new MapLayer();
            var vertices = new Vertex[_mapData.MapSize.X * _mapData.MapSize.Y * TileVertices];

            var verticeIndex = 0;
            foreach (TmxLayerTile tileItem in layer.Tiles)
            {
                AddTileVertices(vertices, verticeIndex, tileItem, tilesetColumns);
                verticeIndex += TileVertices;
            }

            mapLayer.VerticesLayer = vertices;
            _mapLayers.Add(mapLayer);
        }
    }

    public void Update(float deltaTime) { }

    private void DrawVertices(RenderTexture target, RenderStates states, uint tilesCount, Vertex[] vertices, int lineNumber)
    {
        var start = (uint)(lineNumber * _mapData.MapSize.X * TileVertices);

        if (start >= (uint)vertices.Length)
            return;

        var available = (uint)vertices.Length - start;
        var count     = Math.Min(tilesCount, available);

        // Single GPU draw call for the entire visible row
        target.Draw(vertices, start, count, PrimitiveType.Triangles, states);
    }

    private void AddTileVertices(Vertex[] vertices, int verticeIndex, TmxLayerTile tileItem, int tilesetColumns)
    {
        if (tileItem.Gid == 0)
            return; // empty tile – leave vertices at default (transparent)

        var xIndex = (tileItem.Gid - 1) % tilesetColumns;
        var yIndex = (tileItem.Gid - 1) / tilesetColumns;

        AddTileVertices(vertices, verticeIndex, xIndex, yIndex, new Vector2f(tileItem.X, tileItem.Y),
            tileItem.VerticalFlip, tileItem.HorizontalFlip, tileItem.DiagonalFlip);
    }

    private unsafe void AddTileVertices(Vertex[] vertices, int verticeIndex, int x, int y, Vector2f position,
        bool verticalFlip = false, bool horizontalFlip = false, bool diagonalFlip = false)
    {
        var tileWorldDimension = GetWorldTileSize.X * _mapData.TileWorldDimension;
        var tw = (float)GetWorldTileSize.X;
        var th = (float)GetWorldTileSize.Y;

        // Texture corners: top-left, top-right, bottom-right, bottom-left
        var t0 = new Vector2f(tw * x,      th * y);
        var t1 = new Vector2f(tw * x + tw, th * y);
        var t2 = new Vector2f(tw * x + tw, th * y + th);
        var t3 = new Vector2f(tw * x,      th * y + th);

        if (horizontalFlip) { (t0, t1, t2, t3) = (t1, t0, t3, t2); }
        if (verticalFlip)   { (t0, t1, t2, t3) = (t3, t2, t1, t0); }
        if (diagonalFlip)   { (t0, t1, t2, t3) = (t1, t2, t3, t0); }

        // World-space corners
        var p0 = position                        * tileWorldDimension;
        var p1 = (position + new Vector2f(1, 0)) * tileWorldDimension;
        var p2 = (position + new Vector2f(1, 1)) * tileWorldDimension;
        var p3 = (position + new Vector2f(0, 1)) * tileWorldDimension;

        fixed (Vertex* fptr = vertices)
        {
            var ptr = fptr + verticeIndex;

            // Triangle 1: top-left, top-right, bottom-right
            ptr->Position = p0; ptr->TexCoords = t0; ptr->Color = Color.White; ptr++;
            ptr->Position = p1; ptr->TexCoords = t1; ptr->Color = Color.White; ptr++;
            ptr->Position = p2; ptr->TexCoords = t2; ptr->Color = Color.White; ptr++;

            // Triangle 2: top-left, bottom-right, bottom-left
            ptr->Position = p0; ptr->TexCoords = t0; ptr->Color = Color.White; ptr++;
            ptr->Position = p2; ptr->TexCoords = t2; ptr->Color = Color.White; ptr++;
            ptr->Position = p3; ptr->TexCoords = t3; ptr->Color = Color.White;
        }
    }

    private Vector2i GetViewDownRight(Vector2f size, Vector2f translation)
        => new((int)(translation + size).X / GetWorldTileSize.X, (int)(translation + size).Y / GetWorldTileSize.Y);

    private Vector2i GetViewTopLeft(Vector2f translation)
        => new((int)translation.X / GetWorldTileSize.X, (int)translation.Y / GetWorldTileSize.Y);

    private Vector2f GetViewSize(View view)   => view.Size   / _mapData.TileWorldDimension;
    private Vector2f GetViewCenter(View view) => view.Center / _mapData.TileWorldDimension;
    private Vector2i GetWorldTileSize => _mapData.TileSize;
}