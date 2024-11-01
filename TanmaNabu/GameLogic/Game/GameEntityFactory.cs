using SFML.System;
using TanmaNabu.Core.DataStructures;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Map;
using TanmaNabu.GameLogic.Game.Exceptions;

namespace TanmaNabu.GameLogic.Game;

public static class GameEntityFactory
{
    public static GameEntity Get(GameContext context, GameEntityType gameEntityType, int positionX, int positionY, MapData mapData)
    {
        return gameEntityType switch
        {
            GameEntityType.Player => CreatePlayerEntity(context, new Vector2f(positionX, positionY), mapData),
            _ => throw new GameInvalidEnumArgumentException("Unrecognized GameEntityType value", gameEntityType.AllowedValues())
        };
    }

    public static void AddAllEntities(GameContext context, MapData mapData)
    {
        foreach (var item in mapData.MapEntities)
        {
            CreateEntity(context, item, mapData);
        }
    }

    private static void CreateEntity(GameContext context, MapEntity mapEntity, MapData mapData)
    {
        var entity = context.CreateEntity();
        entity.AddDebugMessage($"Hello: {mapEntity.Name}, Type: {mapEntity.Type}");

        if (mapEntity.IsPlayer)
        {
            entity.IsPlayer = true;
        }

        var position = new Point2(mapEntity.X + mapEntity.Width / 2, mapEntity.Y + mapEntity.Height / 2) * mapData.TileWorldDimension;
        entity.AddPosition(position.X, position.Y);

        entity.AddAnimationType(mapEntity.InitialState);
        entity.AddAnimation(mapEntity.TilesetName, mapEntity.Type, mapData.SpriteWorldDimension);
        entity.AddCollision(mapEntity.TilesetName, mapEntity.Type, mapData.SpriteWorldDimension);
        entity.AddMovement(mapEntity.MovementSpeed);
        entity.AddCharacter(mapEntity.Type);
    }

    private static GameEntity CreatePlayerEntity(GameContext context, Vector2f position, MapData mapData)
    {
        var player = context.CreateEntity();
        player.IsPlayer = true;
        player.AddDebugMessage("Hello player 1");
        player.AddPosition(position.X, position.Y);

        return player;
    }
}