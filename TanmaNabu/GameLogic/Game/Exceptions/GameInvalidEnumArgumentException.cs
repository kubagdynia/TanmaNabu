using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.GameLogic.Game.Exceptions;

public class GameInvalidEnumArgumentException(string message, string hint)
    : BaseCoreInvalidEnumArgumentException(message, hint);