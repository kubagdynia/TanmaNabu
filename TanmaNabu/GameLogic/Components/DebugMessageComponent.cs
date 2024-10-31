using Entitas;

namespace TanmaNabu.GameLogic.Components;

public sealed class DebugMessageComponent : IComponent
{
    public string Message;

    public override string ToString() => $"DebugMessage({Message})";
}