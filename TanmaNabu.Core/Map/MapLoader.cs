using SFML.Graphics;
using SFML.System;
using System;
using System.IO;
using System.Linq;
using TanmaNabu.Core.Animation;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Managers;
using TanmaNabu.Core.Map.Exceptions;
using TanmaNabu.Core.Settings;
using TiledSharp;

namespace TanmaNabu.Core.Map
{
    public static class MapLoader
    {
        public static void LoadMap(string mapResourceName, MapData data)
        {
#if DEBUG
            "Map loading...".Log(true);
#endif
            var map = AssetManager.Map.Get(mapResourceName);

            // Load resources
            LoadMapProperties(map, data);
            LoadTileLayers(map, data);
            LoadObjectLayers(map, data);
            LoadMapTileset(map, data);

            UpdateData(data);
#if DEBUG
            "Map loaded".Log();
#endif
        }

        private static void UpdateData(MapData data)
        {
            data.MapRec = new FloatRect(0, 0, data.TileSize.X * data.MapSize.X, data.TileSize.Y * data.MapSize.Y);
        }

        private static void LoadMapProperties(TmxMap map, MapData data)
        {
            data.MapSize = new Vector2i(map.Width, map.Height);
            data.TileSize = new Vector2i(map.TileWidth, map.TileHeight);

            // Background Image Path
            if (GetMapPropertyValue(map, MapCustomProperties.BackgroundImagePath, out string backgroundImagePath))
            {
                data.BackgroundImagePath = backgroundImagePath;
            }

            // Background Music Path
            if (GetMapPropertyValue(map, MapCustomProperties.BackgroundMusicPath, out string backgroundMusicPath))
            {
                data.BackgroundImagePath = backgroundMusicPath;
            }

            // Tilesets Path
            if (GetMapPropertyValue(map, MapCustomProperties.TilesetsPath, out string tilesetsPath))
            {
                data.TilesetsPath = tilesetsPath;
            }
#if DEBUG
                "-->Map properties loaded".Log(true);
#endif
        }

        private static void LoadTileLayers(TmxMap map, MapData data)
        {
#if DEBUG
            $"-->Tile Groups Layers: {map.Groups.Count}".Log();
#endif
            if (!BackgroundTileLayersExists(map))
            {
                throw new MapException("Map file could not be loaded because no background map layers found.", "Add Background layer to the map file.");
            }

            foreach (var group in map.Groups)
            {
#if DEBUG
                $"---->{group.Name}".Log();
#endif
                LoadBackgroundLayer(data, group);
                LoadForegroundLayer(data, group);
                LoadAlwaysFrontLayer(data, group);
            }
#if DEBUG
            string.Empty.Log();
#endif
        }

        private static void LoadMapTileset(TmxMap map, MapData data)
        {
            if (!TilesetsExists(map))
            {
                throw new MapException("Map file could not be loaded because no tileset found.", "Add tileset map file.");
            }

            foreach (var item in map.Tilesets)
            {
                var mapTileset = new MapTileset
                {
                    Firstgid = item.FirstGid,
                    Name = item.Name,
                    TileWidth = item.TileWidth,
                    TileHeight = item.TileHeight,
                    TileCount = item.TileCount ?? 0,
                    Columns = item.Columns ?? 0,
                    ImagePath = item.Image.Source,
                    ImageWidth = item.Image.Width ?? 0,
                    ImageHeight = item.Image.Height ?? 0
                };
                data.Tilesets.Add(mapTileset);
            }
#if DEBUG
            $"-->Tileset loaded: {map.Tilesets.Count}".Log();
            foreach (var item in map.Tilesets)
            {
                $"----> Name: {item.Name}, Tiles: {item.TileCount}, Width: {item.TileWidth}, Height: {item.TileHeight}, Image: {Path.GetFileName(item.Image.Source)}".Log();
            }
            string.Empty.Log();
#endif
        }

        private static void LoadBackgroundLayer(MapData data, TmxGroup group)
        {
            if (!group.Name.Equals(MapLayers.Background.ToString() , StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
#if DEBUG
            $"------>Tile Layers loaded: {group.TileLayers.Count}".Log();
#endif
            foreach (var layer in group.TileLayers)
            {
#if DEBUG
                $"-------->Name: {layer.Name}, Tiles: {layer.Tiles.Count}, Tiles affected: {layer.Tiles.Count(c => c.Gid != 0)}".Log();
#endif
                data.BackgroundTileLayers.Add(layer);
            }
        }

        private static void LoadForegroundLayer(MapData data, TmxGroup group)
        {
            if (!group.Name.Equals(MapLayers.Foreground.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
#if DEBUG
            $"------>Tile Layers loaded: {group.TileLayers.Count}".Log();
#endif
            foreach (var layer in group.TileLayers)
            {
#if DEBUG
                $"-------->Name: {layer.Name}, Tiles: {layer.Tiles.Count}, Tiles affected: {layer.Tiles.Count(c => c.Gid != 0)}".Log();
#endif
                data.ForegroundTileLayers.Add(layer);
            }
        }

        private static void LoadAlwaysFrontLayer(MapData data, TmxGroup group)
        {
            if (!group.Name.Equals(MapLayers.AlwaysFront.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
#if DEBUG
            $"------>Tile Layers loaded: {group.TileLayers.Count}".Log();
#endif
            foreach (var layer in group.TileLayers)
            {
#if DEBUG
                $"-------->Name: {layer.Name}, Tiles: {layer.Tiles.Count}, Tiles affected: {layer.Tiles.Count(c => c.Gid != 0)}".Log();
#endif
                data.AlwaysFrontTileLayers.Add(layer);
            }
        }

        private static void LoadObjectLayers(TmxMap map, MapData data)
        {
#if DEBUG
            $"-->Objects Layers: {map.ObjectGroups.Count}".Log();
#endif
            foreach (var objectGroup in map.ObjectGroups)
            {
#if DEBUG
                $"---->{objectGroup.Name}".Log();
#endif
                LoadCollidersLayer(data, objectGroup);
                LoadEntitiesLayer(data, objectGroup);
                //TODO: load Objects and Entities
            }
#if DEBUG
            string.Empty.Log();
#endif
        }

        private static void LoadCollidersLayer(MapData data, TmxObjectGroup objectGroup)
        {
            if (!ObjectGroupExists(objectGroup, MapLayers.Colliders.ToString()) || !ObjectGroupHasObjects(objectGroup))
            {
                return;
            }

            data.CollidersLayer.Name = objectGroup.Name;
            data.CollidersLayer.Visible = objectGroup.Visible;
            data.CollidersLayer.Opacity = objectGroup.Opacity;

            foreach (var value in objectGroup.Objects)
            {
                var collider = new DataStructures.IntRect(
                    value.X, value.Y, value.X + value.Width, value.Y + value.Height);

                collider *= data.TileWorldDimension;

                data.CollidersLayer.Colliders.Add(collider);
            }
#if DEBUG
            $"------>Objects: {objectGroup.Objects.Count}".Log();
#endif
        }

        private static void LoadEntitiesLayer(MapData data, TmxObjectGroup objectGroup)
        {
            if (!ObjectGroupExists(objectGroup, MapLayers.Entities.ToString()) || !ObjectGroupHasObjects(objectGroup))
            {
                return;
            }

            foreach (TmxObject entityObject in objectGroup.Objects)
            {
                MapEntity mapEntity = new MapEntity
                {
                    Name = entityObject.Name,
                    Type = entityObject.Type,
                    Visible = entityObject.Visible,
                    X = (int)entityObject.X,
                    Y = (int)entityObject.Y,
                    Width = (int)entityObject.Width,
                    Height = (int)entityObject.Height
                };

                LoadEntityCustomProperties(entityObject, mapEntity);

                data.MapEntities.Add(mapEntity);
            }
        }

        private static void LoadEntityCustomProperties(TmxObject entityObject, MapEntity mapEntity)
        {
            if (entityObject.Properties == null) return;

            // Initial State
            if (GetPropertyValue(entityObject, EntityCustomProperties.InitialState, out string initialState))
            {
                if (Enum.TryParse(initialState, true, out AnimationType value))
                {
                    mapEntity.InitialState = value;
                }
            }

            // Movement Speed
            if (GetPropertyValue(entityObject, EntityCustomProperties.MovementSpeed, out string movementSpeed))
            {
                if (int.TryParse(movementSpeed, out int value))
                {
                    mapEntity.MovementSpeed = value;
                }
            }

            // Tileset Name
            if (GetPropertyValue(entityObject, EntityCustomProperties.TilesetName, out string tilesetName))
            {
                // Load tileset
                AssetManager.Tileset.Load(tilesetName, GameSettings.GetFullPath(
                    SettingsPropertyType.TilesetsPath, $"{tilesetName}{GameSettings.TilesetFileExtension}"));

                mapEntity.TilesetName = tilesetName;
            }

            // Object Type
            if (GetPropertyValue(entityObject, EntityCustomProperties.ObjectType, out string objectType))
            {
                mapEntity.ObjectType = objectType;
            }

            // Is Player
            if (GetPropertyValue(entityObject, EntityCustomProperties.IsPlayer, out string isPLayer))
            {
                if (bool.TryParse(isPLayer, out bool isPlayer))
                {
                    mapEntity.IsPlayer = isPlayer;
                }
            }
        }

        private static bool GetPropertyValue(TmxObject entityObject, EntityCustomProperties entityCustomProperty, out string propertyValue)
        {
            var entityProperty =
                entityObject.Properties.SingleOrDefault(c => c.Key.EqualsIgnoreCase(entityCustomProperty.ToString()));

            if (entityProperty.Key == null)
            {
                propertyValue = null;
                return false;
            }

            propertyValue = entityProperty.Value;
            return true;
        }

        private static bool GetMapPropertyValue(TmxMap map, MapCustomProperties mapCustomProperty, out string propertyValue)
        {
            if (map.Properties == null)
            {
                propertyValue = null;
                return false;
            }

            var mapProperty = map.Properties.SingleOrDefault(c => c.Key.EqualsIgnoreCase(mapCustomProperty.ToString()));

            if (mapProperty.Key == null)
            {
                propertyValue = null;
                return false;
            }

            propertyValue = mapProperty.Value;
            return true;
        }

        private static bool BackgroundTileLayersExists(TmxMap map)
        {
            return map.Groups != null && map.Groups.Any(x => x.Name.EqualsIgnoreCase(MapLayers.Background.ToString()));
        }

        private static bool TilesetsExists(TmxMap map)
        {
            return map.Tilesets != null && map.Tilesets.Any();
        }

        private static bool ObjectGroupExists(TmxObjectGroup objectGroup, string name)
        {
            return objectGroup != null && name.EqualsIgnoreCase(objectGroup.Name);
        }

        private static bool ObjectGroupHasObjects(TmxObjectGroup objectGroup)
        {
            return objectGroup?.Objects != null && objectGroup.Objects.Any();
        }
    }
}
