using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game
{
    public partial class GameEntity
    {
        public AnimationComponent Animation => (AnimationComponent)GetComponent(GameComponentsLookup.Animation);
        public bool HasAnimation => HasComponent(GameComponentsLookup.Animation);

        public void AddAnimation(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateAnimationComponent(tilesetName, objectType, spriteWorldDimension);
            AddComponent(GameComponentsLookup.Animation, component);
        }

        public void ReplaceAnimation(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateAnimationComponent(tilesetName, objectType, spriteWorldDimension);
            ReplaceComponent(GameComponentsLookup.Animation, component);
        }

        public void RemoveAnimation()
        {
            RemoveComponent(GameComponentsLookup.Animation);
        }

        private IComponent CreateAnimationComponent(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateComponent<AnimationComponent>(GameComponentsLookup.Animation);
            component.SetTileset(tilesetName, objectType, spriteWorldDimension);

            return component;
        }
    }

    public static partial class GameMatcher
    {
        private static IMatcher<GameEntity> _matcherAnimation;

        public static IMatcher<GameEntity> Animation
        {
            get
            {
                if (_matcherAnimation == null)
                {
                    var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Animation);
                    matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                    _matcherAnimation = matcher;
                }

                return _matcherAnimation;
            }
        }
    }
}
