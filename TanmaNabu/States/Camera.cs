using SFML.Graphics;
using SFML.System;
using System;
using TanmaNabu.Core.Game;
using TanmaNabu.GameLogic;
using TanmaNabu.Core.Extensions;

namespace TanmaNabu.States;

public class Camera(RenderTarget renderTarget, Contexts contexts)
{
    private Vector2f _currentPosition = renderTarget.GetView().Center;
    private float _currentZoomFactor;

    private const float MoveSpeed = 0.000008f;

    public void Update(float deltaTime, GameTime gameTime, float positionX, float positionY)
    {
        float lerpSpeed = CameraMath.Clamp(gameTime.ElapsedTime.AsMicroseconds() * MoveSpeed, 0, 1);

        var view = new View
        {
            Size = new Vector2f(renderTarget.Size.X, renderTarget.Size.Y),
            Viewport = new FloatRect(0f, 0f, 1.0f, 1.0f),
        };
        view.Zoom(contexts.GameMap.MapData.MapZoomFactor);

        var targetCenter = TargetCenter(positionX, positionY);

        var oldPosition = _currentPosition;

        _currentPosition = CameraMath.Lerp(new Vector2f(targetCenter.X, targetCenter.Y), _currentPosition, lerpSpeed);

        if (oldPosition.Equals(_currentPosition, 0.1f) && Math.Abs(_currentZoomFactor - contexts.GameMap.MapData.MapZoomFactor) < 0.01f)
        {
            return;
        }

        _currentZoomFactor = contexts.GameMap.MapData.MapZoomFactor;

        // How to fix vertical artifact lines in a vertex array in SFML, WITH pixel perfect zoom/move?
        // https://stackoverflow.com/questions/55997965/how-to-fix-vertical-artifact-lines-in-a-vertex-array-in-sfml-with-pixel-perfect
        // https://www.sfml-dev.org/tutorials/2.5/graphics-draw.php#off-screen-drawing

        _currentPosition.X = (float)Math.Floor(_currentPosition.X);
        _currentPosition.Y = (float)Math.Floor(_currentPosition.Y);

        view.Center = _currentPosition;

        renderTarget.SetView(view);
    }

    private (float X, float Y) TargetCenter(float positionX, float positionY)
    {
        var targetCenterX = Math.Max(
            renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
            Math.Min(
                contexts.GameMap.MapData.MapRec.Width * contexts.GameMap.MapData.TileWorldDimension -
                renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionX));

        var targetCenterY = Math.Max(renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
            Math.Min(
                contexts.GameMap.MapData.MapRec.Height * contexts.GameMap.MapData.TileWorldDimension -
                renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionY));
        
        return (targetCenterX, targetCenterY);
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