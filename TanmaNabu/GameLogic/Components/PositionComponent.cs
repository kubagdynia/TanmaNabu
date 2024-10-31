using Entitas;

namespace TanmaNabu.GameLogic.Components;

public sealed class PositionComponent : IComponent
{
    public float X { get; set; }
    public float Y { get; set; }

    public override string ToString()
    {
        return $"Position({X}, {Y})";
    }
}