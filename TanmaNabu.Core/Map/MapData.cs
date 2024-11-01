﻿using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TiledSharp;

namespace TanmaNabu.Core.Map;

public class MapData
{
    public int TileWorldDimension { get; } = 2;

    public int SpriteWorldDimension { get; } = 3;

    public int CollisionNearbyDistance { get; } = 20;

    private float _mapZoomFactor = 0.6f;
    public float MapZoomFactor
    {
        get => _mapZoomFactor;
        set => _mapZoomFactor = value > 0.01f ? value : 0.01f;
    }

    public Vector2i MapSize { get; set; }
    public Vector2i TileSize { get; set; }

    public FloatRect MapRec { get; set; }

    public string BackgroundImagePath { get; set; }
    public string BackgroundMusicPath { get; set; }
    public string TilesetsPath { get; set; }

    private List<MapTileset> _tilesets;
    public List<MapTileset> Tilesets => _tilesets ??= [];

    private List<TmxLayer> _backgroundTileLayers;
    public List<TmxLayer> BackgroundTileLayers => _backgroundTileLayers ??= [];

    private List<TmxLayer> _foregroundTileLayers;
    public List<TmxLayer> ForegroundTileLayers => _foregroundTileLayers ??= [];

    private List<TmxLayer> _alwaysFrontTileLayers;
    public List<TmxLayer> AlwaysFrontTileLayers => _alwaysFrontTileLayers ??= [];

    private CollidersLayer _collidersLayer;
    public CollidersLayer CollidersLayer => _collidersLayer ??= new CollidersLayer();

    private List<MapEntity> _mapEntities;
    public List<MapEntity> MapEntities => _mapEntities ??= [];

    public IEnumerable<DataStructures.IntRect> GetCollisionsNearby(DataStructures.IntRect target, int distance)
    {
        foreach (var collider in CollidersLayer.Colliders)
        {
            if (collider.Right > target.Left - distance && collider.Left < target.Right + distance &&
                collider.Bottom > target.Top - distance && collider.Top < target.Bottom + distance)
            {
                yield return collider;
            }
        }
    }
}