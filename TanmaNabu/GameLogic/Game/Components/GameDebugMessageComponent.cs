using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public partial class GameEntity
{
    public DebugMessageComponent DebugMessage => (DebugMessageComponent)GetComponent(GameComponentsLookup.DebugMessage);
    public bool HasDebugMessage => HasComponent(GameComponentsLookup.DebugMessage);

    public void AddDebugMessage(string message)
    {
        var index = GameComponentsLookup.DebugMessage;
        var component = CreateComponent<DebugMessageComponent>(index);
        component.Message = message;
        AddComponent(index, component);
    }

    public void ReplaceDebugMessage(string message)
    {
        var index = GameComponentsLookup.DebugMessage;
        var component = CreateComponent<DebugMessageComponent>(index);
        component.Message = message;
        ReplaceComponent(index, component);
    }

    public void RemoveDebugMessage()
    {
        RemoveComponent(GameComponentsLookup.DebugMessage);
    }
}

public static partial class GameMatcher
{
    private static IMatcher<GameEntity> _matcherDebugMessage;

    public static IMatcher<GameEntity> DebugMessage
    {
        get
        {
            if (_matcherDebugMessage == null)
            {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.DebugMessage);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherDebugMessage = matcher;
            }

            return _matcherDebugMessage;
        }
    }
}