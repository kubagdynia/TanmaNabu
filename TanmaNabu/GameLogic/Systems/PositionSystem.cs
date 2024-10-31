using Entitas;
using System.Collections.Generic;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class PositionSystem(Contexts contexts) : ReactiveSystem<GameEntity>(contexts.Game)
{
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        // we only care about entities with PositionComponent 
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Animation));
    }

    protected override bool Filter(GameEntity entity)
    {
        // good practice to perform a final check in case 
        // the entity has been altered in a different system.
        return entity.HasPosition && entity.HasAnimation;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        // this is the list of entities that meet our conditions
        foreach (var e in entities)
        {
            e.Animation.UpdateSpritePosition(e.Position.X, e.Position.Y);
        }
    }
}