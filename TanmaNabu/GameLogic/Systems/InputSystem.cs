using Entitas;
using SFML.Graphics;
using SFML.Window;
using TanmaNabu.Core.Animation;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class InputSystem : IExecuteSystem
{
    private readonly Contexts _contexts;

    public InputSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    // Runs early every frame
    public void Execute()
    {
        #region ZOOM

        if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp))
        {
            _contexts.GameMap.MapData.MapZoomFactor -= 0.01f;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown))
        {
            _contexts.GameMap.MapData.MapZoomFactor += 0.01f;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Home))
        {
            _contexts.GameMap.MapData.MapZoomFactor = 1;
        }

        #endregion

        #region PLAYER

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

        #endregion
    }

    private void ChangePlayerPosition(float x, float y)
    {
        var entity = _contexts.Game.GetEntity(GameMatcher.Player);

        if (entity.HasMovement)
        {
            if (x != 0 || y != 0)
            {
                int entitySpeed = entity.Movement.Speed;

                x *= _contexts.GameTime.ElapsedTime.AsSeconds() * entitySpeed;
                y *= _contexts.GameTime.ElapsedTime.AsSeconds() * entitySpeed;

                FloatRect spriteRect = entity.Animation.GetSpriteGlobalBounds();
                int tileId = entity.Animation.GetCurrentTiledId();

                if (entity.HasCollision)
                {
                    var spriteCollisionRect = entity.Collision.GetCollisionRectGlobalBounds(tileId, spriteRect, x, y);

                    var collisions = _contexts.GameMap.MapData.GetCollisionsNearby(spriteCollisionRect, _contexts.GameMap.MapData.CollisionNearbyDistance);

                    foreach (Core.DataStructures.IntRect collsionRect in collisions)
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

        if (entity.HasAnimationType)
        {
            AnimationType animType;
            if (x < 0)
            {
                animType = AnimationType.WalkLeft;
            }
            else if (x > 0)
            {
                animType = AnimationType.WalkRight;
            }
            else if (y < 0)
            {
                animType = AnimationType.WalkUp;
            }
            else if (y > 0)
            {
                animType = AnimationType.WalkDown;
            }
            else
            {
                animType = AnimationType.Idle;
            }

            if (entity.AnimationType.AnimationType != animType)
            {
                entity.ReplaceAnimationType(animType);
            }
        }
    }
}