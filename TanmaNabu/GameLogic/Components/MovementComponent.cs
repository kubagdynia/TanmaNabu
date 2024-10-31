using Entitas;

namespace TanmaNabu.GameLogic.Components;

public sealed class MovementComponent : IComponent
{
    public int Speed { get; set; }

    public override string ToString()
    {
        return $"Movement(Speed: {Speed})";
    }
}