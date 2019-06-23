using SFML.Graphics;
using SFML.System;
using System;
using TanmaNabu.GameLogic;

namespace TanmaNabu.States
{
    public class Camera
    {
        protected readonly RenderTarget renderTarget;
        protected readonly Contexts contexts;

        private Vector2f currentPosition;
        private float currentZoomFactor;

        private const float moveSpeed = 0.000008f;

        public Camera(RenderTarget renderTarget, Contexts contexts)
        {
            this.renderTarget = renderTarget;
            this.contexts = contexts;
            currentPosition = renderTarget.GetView().Center;
        }

        public void Update(float deltaTime, Time elapsedTime, float positionX, float positionY)
        {
            float lerpSpeed = CameraMath.Clamp(elapsedTime.AsMicroseconds() * moveSpeed, 0, 1);

            var view = new View
            {
                Size = new Vector2f(renderTarget.Size.X, renderTarget.Size.Y),
                Viewport = new FloatRect(0f, 0f, 1.0f, 1.0f),
            };
            view.Zoom(contexts.GameMap.MapData.MapZoomFactor);

            var targetCenterX = Math.Max(
                renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
                Math.Min(contexts.GameMap.MapData.MapRec.Width * contexts.GameMap.MapData.TileWorldDimension - renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionX));

            var targetCenterY = Math.Max(renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
                Math.Min(contexts.GameMap.MapData.MapRec.Height * contexts.GameMap.MapData.TileWorldDimension - renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionY));

            var oldPosition = currentPosition;

            currentPosition = CameraMath.Lerp(new Vector2f(targetCenterX, targetCenterY), currentPosition, lerpSpeed);

            if (oldPosition.Equals(currentPosition, 0.1f) && currentZoomFactor == contexts.GameMap.MapData.MapZoomFactor)
            {
                return;
            }

            currentZoomFactor = contexts.GameMap.MapData.MapZoomFactor;

            // How to fix vertical artifact lines in a vertex array in SFML, WITH pixel perfect zoom/move?
            // https://stackoverflow.com/questions/55997965/how-to-fix-vertical-artifact-lines-in-a-vertex-array-in-sfml-with-pixel-perfect
            // https://www.sfml-dev.org/tutorials/2.5/graphics-draw.php#off-screen-drawing

            currentPosition.X = (float)Math.Floor(currentPosition.X);
            currentPosition.Y = (float)Math.Floor(currentPosition.Y);

            view.Center = currentPosition;

            renderTarget.SetView(view);
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
