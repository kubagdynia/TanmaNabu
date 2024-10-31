using Entitas;
using System.Collections.Generic;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class AnimationTypeSystem(Contexts contexts) : ReactiveSystem<GameEntity>(contexts.Game)
{
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        // we only care about entities with PositionComponent 
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Animation, GameMatcher.AnimationType));
    }

    protected override bool Filter(GameEntity entity)
    {
        // good practice to perform a final check in case 
        // the entity has been altered in a different system.
        return entity.HasAnimation && entity.HasAnimationType;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        // this is the list of entities that meet our conditions
        foreach (var e in entities)
        {
            e.Animation.UpdateAnimationType(e.AnimationType.AnimationType);
        }
    }
}