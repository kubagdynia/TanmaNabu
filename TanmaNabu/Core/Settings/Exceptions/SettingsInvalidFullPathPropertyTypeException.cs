using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.Core.Settings.Exceptions
{
    public class SettingsInvalidFullPathPropertyTypeException : BaseCoreException
    {
        public SettingsInvalidFullPathPropertyTypeException(string message, string hint)
            : base(message, hint)
        {

        }
    }
}
