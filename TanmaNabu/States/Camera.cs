using SFML.Graphics;
using SFML.System;
using System;
using TanmaNabu.GameLogic;

namespace TanmaNabu.States;

public class Camera(RenderTexture renderTarget, Contexts contexts)
{
    private Vector2f _currentPosition = renderTarget.GetView().Center;

    // Lerp speed – empirically chosen value, expressed in units / fixedStep.
    private const float MoveSpeed = 5.0f;

    // View as a class field eliminates GC allocation every frame.
    private readonly View _view = new();

    /// <summary>
    /// Updates the camera.
    /// </summary>
    /// <param name="alpha">Interpolation value (0..1) passed from Render – ONLY for future interpolation
    /// of object positions; not used for camera lerp calculation.</param>
    /// <param name="fixedStep">Fixed update step (_updateRate) in seconds.</param>
    /// <param name="positionX">Player position X.</param>
    /// <param name="positionY">Player position Y.</param>
    public void Update(float alpha, float fixedStep, float positionX, float positionY)
    {
        // lerpSpeed depends on fixed step – camera moves smoothly regardless of current rendering FPS
        float lerpSpeed = Clamp(fixedStep * MoveSpeed, 0f, 1f);

        _view.Size = new Vector2f(
            renderTarget.Size.X * contexts.GameMap.MapData.MapZoomFactor,
            renderTarget.Size.Y * contexts.GameMap.MapData.MapZoomFactor);
        _view.Viewport = new FloatRect(new Vector2f(0f, 0f), new Vector2f(1.0f, 1.0f));

        (float X, float Y) targetCenter = TargetCenter(positionX, positionY);

        _currentPosition = Lerp(new Vector2f(targetCenter.X, targetCenter.Y), _currentPosition, lerpSpeed);

        // How to fix vertical artifact lines in a vertex array in SFML, WITH pixel perfect zoom/move?
        // https://stackoverflow.com/questions/55997965/how-to-fix-vertical-artifact-lines-in-a-vertex-array-in-sfml-with-pixel-perfect
        // https://www.sfml-dev.org/tutorials/2.5/graphics-draw.php#off-screen-drawing

        _view.Center = new Vector2f(
            (float)Math.Floor(_currentPosition.X),
            (float)Math.Floor(_currentPosition.Y));

        renderTarget.SetView(_view);
    }

    private (float X, float Y) TargetCenter(float positionX, float positionY)
    {
        float targetCenterX = Math.Max(
            renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
            Math.Min(
                contexts.GameMap.MapData.MapRec.Width * contexts.GameMap.MapData.TileWorldDimension -
                renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionX));

        float targetCenterY = Math.Max(renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor,
            Math.Min(
                contexts.GameMap.MapData.MapRec.Height * contexts.GameMap.MapData.TileWorldDimension -
                renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor, positionY));

        return (targetCenterX, targetCenterY);
    }

    private static float Clamp(float value, float min, float max)
        => value < min ? min : value > max ? max : value;

    private static Vector2f Lerp(Vector2f a, Vector2f b, float t)
        => a * t + (1 - t) * b;
}