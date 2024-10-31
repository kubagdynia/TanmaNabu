using Entitas;
using TanmaNabu.Core.Animation;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public partial class GameEntity
{
    public AnimationTypeComponent AnimationType => (AnimationTypeComponent)GetComponent(GameComponentsLookup.AnimationType);
    public bool HasAnimationType => HasComponent(GameComponentsLookup.AnimationType);

    public void AddAnimationType(AnimationType animationType)
    {
        var component = CreateAnimationTypeComponent(animationType);
        AddComponent(GameComponentsLookup.AnimationType, component);
    }

    public void ReplaceAnimationType(AnimationType animationType)
    {
        var component = CreateAnimationTypeComponent(animationType);
        ReplaceComponent(GameComponentsLookup.AnimationType, component);
    }

    public void RemoveAnimationType()
    {
        RemoveComponent(GameComponentsLookup.AnimationType);
    }

    private IComponent CreateAnimationTypeComponent(AnimationType animationType)
    {
        var component = CreateComponent<AnimationTypeComponent>(GameComponentsLookup.AnimationType);
        component.AnimationType = animationType;

        return component;
    }
}

public static partial class GameMatcher
{
    private static IMatcher<GameEntity> _matcherAnimationType;

    public static IMatcher<GameEntity> AnimationType
    {
        get
        {
            if (_matcherAnimationType == null)
            {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.AnimationType);
                matcher.ComponentNames = GameComponentsLookup.ComponentNames;
                _matcherAnimationType = matcher;
            }

            return _matcherAnimationType;
        }
    }
}