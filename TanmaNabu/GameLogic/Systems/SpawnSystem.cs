using Entitas;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems;

public class SpawnSystem(Contexts contexts) : IInitializeSystem
{
    public void Initialize()
    {
        GameEntityFactory.AddAllEntities(contexts.Game, contexts.GameMap.MapData);
    }
}