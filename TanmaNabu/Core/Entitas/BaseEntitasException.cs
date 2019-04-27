using System;

namespace Entitas
{
    /// Base exception used by Entitas.
    public class BaseEntitasException : Exception
    {
        public BaseEntitasException(string message, string hint)
            : base(hint != null ? $"{message}\n{hint}" : message)
        {
        }
    }
}