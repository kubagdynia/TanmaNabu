using Entitas;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic.Systems
{
    class AnimationSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;

        public AnimationSystem(Contexts contextses)
        {
            _contexts = contextses;
        }

        public void Execute()
        {
            var animations = _contexts.Game.GetEntities(GameMatcher.Animation);

            foreach (var animation in animations)
            {
                animation.Animation.UpdateAnimation(_contexts.Game.DeltaTime);
            }
        }
    }
}
