using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using TanmaNabu.Core.DataStructures;
using TanmaNabu.Core.Managers;
using TiledSharp;

namespace TanmaNabu.GameLogic.Components;

public sealed class CollisionComponent : IComponent
{
    private string _tilesetName;

    private string _objectType;

    private int _spriteWorldDimension;

    private List<CollisionObjectGroup> _collisionObjectGroups;

    public List<CollisionObjectGroup> CollisionObjectGroups => _collisionObjectGroups ??= new List<CollisionObjectGroup>();

    public void SetTileset(string tilesetName, string objectType, int spriteWorldDimension)
    {
        _tilesetName = tilesetName;
        _objectType = objectType;
        _spriteWorldDimension = spriteWorldDimension;
        InitializeCollisions(objectType);
    }

    public CollisionObjectGroup GetCollisionObjectGroup(int tileId)
    {
        return CollisionObjectGroups.SingleOrDefault(c => c.TileId == tileId);
    }

    public IntRect GetFirstCollisionRect(int tileId)
    {
        var collisionObjectGroup = GetCollisionObjectGroup(tileId);

        if (collisionObjectGroup?.Collissions == null)
        {
            return IntRect.Zero;
        }

        return collisionObjectGroup.Collissions[0].CollisionRect;
    }

    public IntRect GetCollisionRectGlobalBounds(int tileId, SFML.Graphics.FloatRect parentRect, float offsetX, float offsetY)
    {
        var collisionRect = GetFirstCollisionRect(tileId);

        var intRect = new IntRect(
            parentRect.Left + collisionRect.Left + offsetX,
            parentRect.Top + collisionRect.Top + offsetY,
            parentRect.Left + collisionRect.Left + offsetX + collisionRect.Width,
            parentRect.Top + collisionRect.Top + offsetY + collisionRect.Height);

        return intRect;
    }

    private bool InitializeCollisions(string objectType)
    {
        if (string.IsNullOrEmpty(_tilesetName))
        {
            return false;
        }

        var tileset = AssetManager.Tileset.Get(_tilesetName);

        if (tileset?.Tiles == null)
        {
            return false;
        }

        // If the objectType is not null than take only the tiles that correspond to that objectType
        List<TmxTilesetTile> tiles;
        if (objectType == null)
        {
            tiles = tileset.Tiles.Where(c => c.ObjectGroups != null && c.ObjectGroups.Any()).ToList();
        }
        else
        {
            tiles = tileset.Tiles.Where(c => c.Type != null && c.ObjectGroups != null && c.ObjectGroups.Any() &&
                                             c.Type.Equals(objectType, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        foreach (var tile in tiles)
        {
            var visibleObjectGroups = tile.ObjectGroups.Where(c => c.Visible);

            if (visibleObjectGroups == null || !visibleObjectGroups.Any())
            {
                continue;
            }

            var collisionObjectGroup = new CollisionObjectGroup
            {
                TileId = tile.Id
            };

            foreach (var objectGroup in visibleObjectGroups)
            {
                // Load weight of the object
                if (objectGroup.Properties != null)
                {
                    var weightValue =
                        objectGroup.Properties.FirstOrDefault(c => c.Key.Equals("weight", StringComparison.OrdinalIgnoreCase)).Value;

                    if (int.TryParse(weightValue, out int weight))
                    {
                        collisionObjectGroup.Weight = weight;
                    }
                }

                // Load collissions
                foreach (var objectValue in objectGroup.Objects)
                {
                    var collisionObject = new CollisionObject
                    {
                        Id = objectValue.Id,
                        CollisionRect = new IntRect(
                            objectValue.X,
                            objectValue.Y,
                            objectValue.X + objectValue.Width,
                            objectValue.Y + objectValue.Height) * _spriteWorldDimension
                    };

                    collisionObjectGroup.Collissions.Add(collisionObject);
                }
            }

            CollisionObjectGroups.Add(collisionObjectGroup);
        }

        return true;
    }
}