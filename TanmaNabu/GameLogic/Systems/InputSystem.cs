using Entitas;
using SFML.Graphics;
using SFML.Window;
using TanmaNabu.Core.Animation;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class InputSystem(Contexts contexts) : IExecuteSystem
{
    // Runs early every frame
    public void Execute()
    {
        Zoom();
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A) ||
            Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D) ||
            Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.W) ||
            Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S))
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                ChangePlayerPosition(-1, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                ChangePlayerPosition(1, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                ChangePlayerPosition(0, -1);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                ChangePlayerPosition(0, 1);
            }
        }
        else
        {
            ChangePlayerPosition(0, 0);
        }
    }

    private void Zoom()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp))
        {
            contexts.GameMap.MapData.MapZoomFactor -= 0.01f;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown))
        {
            contexts.GameMap.MapData.MapZoomFactor += 0.01f;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Home))
        {
            contexts.GameMap.MapData.MapZoomFactor = 1;
        }
    }

    private void ChangePlayerPosition(float x, float y)
    {
        var entity = contexts.Game.GetEntity(GameMatcher.Player);

        if (entity.HasMovement)
        {
            if (x != 0 || y != 0)
            {
                var entitySpeed = entity.Movement.Speed;

                x *= contexts.GameTime.ElapsedTime.AsSeconds() * entitySpeed;
                y *= contexts.GameTime.ElapsedTime.AsSeconds() * entitySpeed;

                var spriteRect = entity.Animation.GetSpriteGlobalBounds();
                var tileId = entity.Animation.GetCurrentTiledId();

                if (entity.HasCollision)
                {
                    var spriteCollisionRect = entity.Collision.GetCollisionRectGlobalBounds(tileId, spriteRect, x, y);

                    var collisions = contexts.GameMap.MapData.GetCollisionsNearby(spriteCollisionRect, contexts.GameMap.MapData.CollisionNearbyDistance);

                    foreach (var collsionRect in collisions)
                    {
                        if (collsionRect.Intersects(spriteCollisionRect))
                        {
                            return;
                        }
                    }
                }

                entity.ReplacePosition(entity.Position.X + x, entity.Position.Y + y);
            }
        }

        if (!entity.HasAnimationType) return;
        
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
}