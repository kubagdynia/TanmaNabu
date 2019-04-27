using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.GameLogic.Game.Exceptions
{
    public class GameException : BaseCoreException
    {
        protected GameException(string message, string hint)
            : base(message, hint)
        {
        }
    }
}
