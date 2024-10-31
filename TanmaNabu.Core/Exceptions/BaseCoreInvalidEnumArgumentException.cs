using System.ComponentModel;

namespace TanmaNabu.Core.Exceptions;

public class BaseCoreInvalidEnumArgumentException : InvalidEnumArgumentException
{
    protected BaseCoreInvalidEnumArgumentException(string message, string hint)
        : base(hint != null ? ($"{message}\n{hint}") : message)
    {

    }
}