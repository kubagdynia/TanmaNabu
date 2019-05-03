using Entitas;
using System;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game
{
    public partial class GameEntity
    {
        public CharacterComponent Character => (CharacterComponent)GetComponent(GameComponentsLookup.Character);
        public bool HasCharacter => HasComponent(GameComponentsLookup.Character);

        public void AddCharacter(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return;

            if (Enum.TryParse(type, true, out ObjectType objectType))
            {
                AddCharacter(objectType);
            }
        }

        public void AddCharacter(ObjectType objectType)
        {
            var component = CreateCharacterComponent(objectType);
            AddComponent(GameComponentsLookup.Character, component);
        }

        public void ReplaceCharacter(ObjectType objectType)
        {
            var component = CreateCharacterComponent(objectType);
            ReplaceComponent(GameComponentsLookup.Character, component);
        }

        public void RemoveCharacter()
        {
            RemoveComponent(GameComponentsLookup.Character);
        }

        private IComponent CreateCharacterComponent(ObjectType objectType)
        {
            var component = CreateComponent<CharacterComponent>(GameComponentsLookup.Character);
            component.ObjectType = objectType;

            return component;
        }
    }

    public static partial class GameMatcher
    {
        private static IMatcher<GameEntity> _matcherCharacter;

        public static IMatcher<GameEntity> Character
        {
            get
            {
                if (_matcherCharacter == null)
                {
                    var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Character);
                    matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                    _matcherCharacter = matcher;
                }

                return _matcherCharacter;
            }
        }
    }
}
