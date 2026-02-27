namespace TanmaNabu.GameLogic.Game;

/// <summary>
/// Input snapshot captured ONCE per render frame, before the fixed-update loop.
/// Multiple InputSystem calls within the same frame all read the same snapshot,
/// preventing the player from jumping farther during momentary slowdowns.
/// </summary>
public class InputState
{
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public float ZoomDelta { get; set; }
    public bool ZoomReset { get; set; }
}
