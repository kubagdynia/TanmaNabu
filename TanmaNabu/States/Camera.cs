using SFML.Graphics;
using SFML.System;
using System;
using TanmaNabu.GameLogic;

namespace TanmaNabu.States
{
    public class Camera
    {
        protected readonly RenderWindow window;
        protected readonly Contexts contexts;

        private Vector2f currentPosition;
        private float currentZoomFactor;

        private const float moveSpeed = 0.000005f;

        public Camera(RenderWindow window, Contexts contexts)
        {
            this.window = window;
            this.contexts = contexts;
            currentPosition = window.GetView().Center;
        }

        public void Update(float deltaTime, Time elapsedTime, float positionX, float positionY)
        {
            float lerpSpeed = CameraMath.Clamp(elapsedTime.AsMicroseconds() * moveSpeed, 0, 1);

            var view = new View
            {
                Size = new Vector2f(window.Size.X, window.Size.Y),
                Viewport = new FloatRect(0f, 0f, 1.0f, 1.0f),
            };
            view.Zoom(contexts.GameMap.MapData.MapZoomFactor);

            var targetCenterX = Math.Max(
                window.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
                Math.Min(contexts.GameMap.MapData.MapRec.Width * contexts.GameMap.MapData.TileWorldDimension - window.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionX));

            var targetCenterY = Math.Max(window.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
                Math.Min(contexts.GameMap.MapData.MapRec.Height * contexts.GameMap.MapData.TileWorldDimension - window.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionY));

            var oldPosition = currentPosition;

            currentPosition = CameraMath.Lerp(new Vector2f(targetCenterX, targetCenterY), currentPosition, lerpSpeed);

            if (oldPosition.Equals(currentPosition, 0.1f) && currentZoomFactor == contexts.GameMap.MapData.MapZoomFactor)
            {
                return;
            }

            currentZoomFactor = contexts.GameMap.MapData.MapZoomFactor;

            view.Center = currentPosition;

            window.SetView(view);
        }
    }

    internal static class CameraMath
    {
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static Vector2f Lerp(Vector2f a, Vector2f b, float t)
        {
            return a * t + (1 - t) * b;
        }
    }
}
