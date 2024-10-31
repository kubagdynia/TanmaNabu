using Entitas;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

class AnimationSystem(Contexts contextses) : IExecuteSystem
{
    public void Execute()
    {
        var animations = contextses.Game.GetEntities(GameMatcher.Animation);

        foreach (var animation in animations)
        {
            animation.Animation.UpdateAnimation(contextses.Game.DeltaTime);
        }
    }
}