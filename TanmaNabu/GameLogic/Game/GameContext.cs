using Entitas;

namespace TanmaNabu.GameLogic.Game
{
    public sealed partial class GameContext : Context<GameEntity>
    {
        public float DeltaTime;

        public GameContext()
            : base(
                GameComponentsLookup.TotalComponents,
                0,
                new ContextInfo(
                    "Game",
                    GameComponentsLookup.ComponentNames,
                    GameComponentsLookup.ComponentTypes
                ),
                (entity) =>
#if (ENTITAS_FAST_AND_UNSAFE)
                    new UnsafeAerc(),
#else
                    new SafeAerc(entity),
#endif
                () => new GameEntity()
            )
        {

        }
    }
}
