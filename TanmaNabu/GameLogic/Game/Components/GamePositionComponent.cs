using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public partial class GameEntity
{
    public PositionComponent Position => (PositionComponent)GetComponent(GameComponentsLookup.Position);
    public bool HasPosition => HasComponent(GameComponentsLookup.Position);

    public void AddPosition(float x, float y)
    {
        IComponent component = CreatePositionComponent(x, y);
        AddComponent(GameComponentsLookup.Position, component);
    }

    public void ReplacePosition(float x, float y)
    {
        IComponent component = CreatePositionComponent(x, y);
        ReplaceComponent(GameComponentsLookup.Position, component);
    }

    public void RemovePosition()
    {
        RemoveComponent(GameComponentsLookup.Position);
    }

    private IComponent CreatePositionComponent(float x, float y)
    {
        PositionComponent component = CreateComponent<PositionComponent>(GameComponentsLookup.Position);
        component.X = x;
        component.Y = y;

        return component;
    }
}

public static partial class GameMatcher
{
    private static IMatcher<GameEntity> _matcherPosition;

    public static IMatcher<GameEntity> Position
    {
        get
        {
            if (_matcherPosition == null)
            {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Position);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherPosition = matcher;
            }

            return _matcherPosition;
        }
    }
}