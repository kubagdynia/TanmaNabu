using Entitas;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

class AnimationSystem(Contexts contexts) : IExecuteSystem
{
    public void Execute()
    {
        var animations = contexts.Game.GetEntities(GameMatcher.Animation);

        foreach (var animation in animations)
        {
            animation.Animation.UpdateAnimation(contexts.Game.DeltaTime);
        }
    }
}