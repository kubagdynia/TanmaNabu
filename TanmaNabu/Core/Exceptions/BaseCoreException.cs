using System;

namespace TanmaNabu.Core.Exceptions
{
    public class BaseCoreException : Exception
    {
        protected BaseCoreException(string message, string hint)
            : base(hint != null ? $"{message}\n{hint}" : message)
        {

        }
    }
}
