using Entitas;
using SFML.Window;
using TanmaNabu.Core.Animation;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems
{
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

            #endregion

            #region PLAYER

            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.Right) ||
                Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    ChangePlayerPosition(-1, 0);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    ChangePlayerPosition(1, 0);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    ChangePlayerPosition(0, -1);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
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

        private void ChangePlayerPosition(int x, int y)
        {
            var entity = _contexts.Game.GetEntity(GameMatcher.Player);

            if (entity.HasMovement)
            {
                if (x != 0 || y != 0)
                {
                    int entitySpeed = entity.Movement.Speed;

                    x *= entitySpeed;
                    y *= entitySpeed;

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
}
