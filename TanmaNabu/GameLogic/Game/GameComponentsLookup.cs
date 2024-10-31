using System;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game;

public static class GameComponentsLookup
{
    public const int DebugMessage = 0;
    public const int Player = 1;
    public const int Position = 2;
    public const int Movement = 3;
    public const int Character = 4;
    public const int AnimationType = 5;
    public const int Animation = 6;
    public const int Collision = 7;

    public const int TotalComponents = 8;

    public static readonly string[] ComponentNames =
    {
        nameof(DebugMessage),
        nameof(Player),
        nameof(Position),
        nameof(Movement),
        nameof(Character),
        nameof(AnimationType),
        nameof(Animation),
        nameof(Collision)
    };

    public static readonly Type[] ComponentTypes =
    {
        typeof(DebugMessageComponent),
        typeof(PlayerComponent),
        typeof(PositionComponent),
        typeof(MovementComponent),
        typeof(CharacterComponent),
        typeof(AnimationTypeComponent),
        typeof(AnimationComponent),
        typeof(CollisionComponent)
    };
}