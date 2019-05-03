using System;
using System.Collections.Generic;
using System.Text;
using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.GameLogic.Game.Exceptions
{
    public class GameInvalidEnumArgumentException : BaseCoreInvalidEnumArgumentException
    {
        public GameInvalidEnumArgumentException(string message, string hint)
            : base(message, hint)
        {

        }
    }
}
