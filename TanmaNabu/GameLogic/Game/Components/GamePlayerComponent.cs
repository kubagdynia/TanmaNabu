using Entitas;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public partial class GameEntity
{
    static readonly PlayerComponent PlayerComponent = new PlayerComponent();

    public bool IsPlayer
    {
        get => HasComponent(GameComponentsLookup.Player);
        set
        {
            if (value == IsPlayer) return;

            if (value)
            {
                AddComponent(GameComponentsLookup.Player, PlayerComponent);
            }
            else
            {
                RemoveComponent(GameComponentsLookup.Player);
            }
        }
    }
}

public static partial class GameMatcher
{
    private static IMatcher<GameEntity> _matcherPlayer;

    public static IMatcher<GameEntity> Player
    {
        get
        {
            if (_matcherPlayer == null)
            {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.Player);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherPlayer = matcher;
            }

            return _matcherPlayer;
        }
    }
}