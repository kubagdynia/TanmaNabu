using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public partial class GameEntity
{
    public MovementComponent Movement => (MovementComponent)GetComponent(GameComponentsLookup.Movement);
    public bool HasMovement => HasComponent(GameComponentsLookup.Movement);

    public void AddMovement(int speed)
    {
        var component = CreateMovementComponent(speed);
        AddComponent(GameComponentsLookup.Movement, component);
    }

    public void ReplaceMovement(int speed)
    {
        var component = CreateMovementComponent(speed);
        ReplaceComponent(GameComponentsLookup.Movement, component);
    }

    public void RemoveMovement()
    {
        RemoveComponent(GameComponentsLookup.Movement);
    }

    private IComponent CreateMovementComponent(int speed)
    {
        var component = CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
        component.Speed = speed;

        return component;
    }
}

public static partial class GameMatcher
{
    private static IMatcher<GameEntity> _matcherMovement;

    public static IMatcher<GameEntity> Movement
    {
        get
        {
            if (_matcherMovement == null)
            {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Movement);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherMovement = matcher;
            }

            return _matcherMovement;
        }
    }
}