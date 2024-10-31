using Entitas;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class SpawnSystem : IInitializeSystem
{
    private readonly Contexts _contexts;

    public SpawnSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        GameEntityFactory.AddAllEntities(_contexts.Game, _contexts.GameMap.MapData);
    }
}