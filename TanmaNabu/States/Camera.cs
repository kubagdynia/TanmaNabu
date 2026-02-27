using SFML.Graphics;
using SFML.System;
using System;
using TanmaNabu.GameLogic;

namespace TanmaNabu.States;

public class Camera(RenderTexture renderTarget, Contexts contexts)
{
    // Lerp speed – camera reaches target in ~1/MoveSpeed fixed steps
    private const float MoveSpeed = 5.0f;

    // View as a class field – eliminates GC allocation every frame
    private readonly View _view = new();

    // Two position states for interpolation in Render()
    private Vector2f _previousPosition = renderTarget.GetView().Center;
    private Vector2f _currentPosition  = renderTarget.GetView().Center;

    /// <summary>
    /// Called every fixed-update step (synchronised with player movement).
    /// Advances the camera towards the player by one lerp step.
    /// </summary>
    public void Tick(float fixedStep, float positionX, float positionY)
    {
        float lerpSpeed = Clamp(fixedStep * MoveSpeed, 0f, 1f);

        var target = TargetCenter(positionX, positionY);

        // Save the previous position before updating
        _previousPosition = _currentPosition;

        // Lerp on raw floats – no Math.Floor here to avoid accumulating rounding error
        _currentPosition = Lerp(
            new Vector2f(target.X, target.Y),
            _currentPosition,
            lerpSpeed);
    }

    /// <summary>
    /// Called in Render() – interpolates between the previous and current position
    /// using alpha (0..1) and applies the view.
    /// </summary>
    public void Apply(float alpha)
    {
        // Interpolate between the previous and current frame position
        var interpolated = _previousPosition + (_currentPosition - _previousPosition) * alpha;

        _view.Size = new Vector2f(
            renderTarget.Size.X * contexts.GameMap.MapData.MapZoomFactor,
            renderTarget.Size.Y * contexts.GameMap.MapData.MapZoomFactor);
        _view.Viewport = new FloatRect(new Vector2f(0f, 0f), new Vector2f(1.0f, 1.0f));

        // Math.Floor only when setting View.Center (pixel-perfect rendering) –
        // does not touch _currentPosition, so rounding error does not accumulate between frames
        _view.Center = new Vector2f(
            (float)Math.Floor(interpolated.X),
            (float)Math.Floor(interpolated.Y));

        renderTarget.SetView(_view);
    }

    private (float X, float Y) TargetCenter(float positionX, float positionY)
    {
        float halfW = renderTarget.Size.X / 2.0f * contexts.GameMap.MapData.MapZoomFactor;
        float halfH = renderTarget.Size.Y / 2.0f * contexts.GameMap.MapData.MapZoomFactor;
        float mapW  = contexts.GameMap.MapData.MapRec.Width  * contexts.GameMap.MapData.TileWorldDimension;
        float mapH  = contexts.GameMap.MapData.MapRec.Height * contexts.GameMap.MapData.TileWorldDimension;

        return (
            Math.Max(halfW, Math.Min(mapW - halfW, positionX)),
            Math.Max(halfH, Math.Min(mapH - halfH, positionY))
        );
    }

    private static float Clamp(float value, float min, float max)
        => value < min ? min : value > max ? max : value;

    private static Vector2f Lerp(Vector2f a, Vector2f b, float t)
        => a * t + (1 - t) * b;
}