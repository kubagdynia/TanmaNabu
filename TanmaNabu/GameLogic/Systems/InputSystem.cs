using System;
using Entitas;
using TanmaNabu.Core.Animation;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class InputSystem(Contexts contexts) : IExecuteSystem
{
    // Runs every fixed-update step – reads from InputState snapshot captured once per render frame.
    public void Execute()
    {
        Zoom();
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        var input = contexts.Game.InputState;
        ChangePlayerPosition(input.MoveX, input.MoveY);
    }

    private void Zoom()
    {
        var input = contexts.Game.InputState;

        if (input.ZoomReset)
        {
            contexts.GameMap.MapData.MapZoomFactor = 1;
        }
        else if (input.ZoomDelta != 0)
        {
            contexts.GameMap.MapData.MapZoomFactor += input.ZoomDelta;
        }
    }

    private void ChangePlayerPosition(float x, float y)
    {
        var entity = contexts.Game.GetEntity(GameMatcher.Player);

        // --- Animation ---
        // Direction from the ORIGINAL input values (before scaling and collision check)
        // so the animation does not freeze when the player hits a wall.
        if (entity.HasAnimationType)
        {
            var animType = x switch
            {
                < 0 => AnimationType.WalkLeft,
                > 0 => AnimationType.WalkRight,
                _ => y switch
                {
                    < 0 => AnimationType.WalkUp,
                    > 0 => AnimationType.WalkDown,
                    _ => AnimationType.Idle
                }
            };

            if (entity.AnimationType.AnimationType != animType)
            {
                entity.ReplaceAnimationType(animType);
            }
        }

        // --- Movement ---
        if (!entity.HasMovement || (x == 0 && y == 0))
        {
            return;
        }

        var speed   = entity.Movement.Speed;
        var scaledX = x * contexts.Game.DeltaTime * speed;
        var scaledY = y * contexts.Game.DeltaTime * speed;

        var spriteRect = entity.Animation.GetSpriteGlobalBounds();
        var tileId     = entity.Animation.GetCurrentTiledId();

        var newX = entity.Position.X;
        var newY = entity.Position.Y;

        if (entity.HasCollision)
        {
            // Collision checked separately per axis – player slides along walls instead of getting stuck
            if (scaledX != 0)
            {
                var rectX    = entity.Collision.GetCollisionRectGlobalBounds(tileId, spriteRect, scaledX, 0);
                var colX     = contexts.GameMap.MapData.GetCollisionsNearby(rectX, contexts.GameMap.MapData.CollisionNearbyDistance);
                var blockedX = false;
                foreach (var col in colX)
                {
                    if (col.Intersects(rectX)) { blockedX = true; break; }
                }
                if (!blockedX) newX += scaledX;
            }

            if (scaledY != 0)
            {
                var rectY    = entity.Collision.GetCollisionRectGlobalBounds(tileId, spriteRect, 0, scaledY);
                var colY     = contexts.GameMap.MapData.GetCollisionsNearby(rectY, contexts.GameMap.MapData.CollisionNearbyDistance);
                var blockedY = false;
                foreach (var col in colY)
                {
                    if (col.Intersects(rectY)) { blockedY = true; break; }
                }
                if (!blockedY) newY += scaledY;
            }
        }
        else
        {
            newX += scaledX;
            newY += scaledY;
        }

        if (MathF.Abs(newX - entity.Position.X) > float.Epsilon ||
            MathF.Abs(newY - entity.Position.Y) > float.Epsilon)
        {
            entity.ReplacePosition(newX, newY);
        }
    }
}