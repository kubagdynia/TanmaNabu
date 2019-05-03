using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game
{
    public partial class GameEntity
    {
        public CollisionComponent Collision => (CollisionComponent)GetComponent(GameComponentsLookup.Collision);
        public bool HasCollision => HasComponent(GameComponentsLookup.Collision);

        public void AddCollision(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateCollisionComponent(tilesetName, objectType, spriteWorldDimension);
            AddComponent(GameComponentsLookup.Collision, component);
        }

        public void ReplaceCollision(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateCollisionComponent(tilesetName, objectType, spriteWorldDimension);
            ReplaceComponent(GameComponentsLookup.Collision, component);
        }

        public void RemoveCollision()
        {
            RemoveComponent(GameComponentsLookup.Collision);
        }

        private IComponent CreateCollisionComponent(string tilesetName, string objectType, int spriteWorldDimension)
        {
            var component = CreateComponent<CollisionComponent>(GameComponentsLookup.Collision);
            component.SetTileset(tilesetName, objectType, spriteWorldDimension);

            return component;
        }
    }

    public static partial class GameMatcher
    {
        private static IMatcher<GameEntity> _matcherCollision;

        public static IMatcher<GameEntity> Collision
        {
            get
            {
                if (_matcherCollision == null)
                {
                    var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Collision);
                    matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                    _matcherCollision = matcher;
                }

                return _matcherCollision;
            }
        }
    }
}
