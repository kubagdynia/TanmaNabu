using Entitas;
using System;
using System.Collections.Generic;
using TanmaNabu.Core.Extensions;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class DebugMessageSystem : ReactiveSystem<GameEntity>, ICleanupSystem
{
    private readonly Contexts _context;
    private readonly IGroup<GameEntity> _debugMessages;

    public DebugMessageSystem(Contexts contexts) : base(contexts.Game)
    {
        _context = contexts;
        _debugMessages = _context.Game.GetGroup(GameMatcher.DebugMessage);
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        // we only care about entities with DebugMessageComponent 
        return context.CreateCollector(GameMatcher.DebugMessage);
    }

    protected override bool Filter(GameEntity entity)
    {
        // good practice to perform a final check in case 
        // the entity has been altered in a different system.
        return entity.HasDebugMessage;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        // this is the list of entities that meet our conditions
        foreach (var e in entities)
        {
            // we can safely access their DebugMessage component
            // then grab the string data and print it
            Console.WriteLine(e.DebugMessage.Message);
        }
    }

    public void Cleanup()
    {
        foreach (var entity in _debugMessages.GetEntities())
        {
            entity.RemoveDebugMessage();
            "Destroy DebugMessage".Log();
        }
    }
}