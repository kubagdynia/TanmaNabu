using Entitas;
using TanmaNabu.Core.Animation;

namespace TanmaNabu.GameLogic.Components;

public sealed class AnimationTypeComponent : IComponent
{
    public AnimationType AnimationType;

    public override string ToString()
    {
        return $"AnimationType({AnimationType})";
    }
}