using System;
using TanmaNabu.GameLogic.Components;

namespace TanmaNabu.GameLogic.Game
{
    public static class GameComponentsLookup
    {
        public const int DebugMessage = 0;

        public const int TotalComponents = 1;

        public static readonly string[] ComponentNames =
        {
            nameof(DebugMessage)
        };

        public static readonly Type[] ComponentTypes =
        {
            typeof(DebugMessageComponent)
        };
    }
}
