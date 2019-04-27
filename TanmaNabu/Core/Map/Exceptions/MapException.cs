using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.Core.Map.Exceptions
{
    public class MapException : BaseCoreException
    {
        public MapException(string message, string hint)
            : base(message, hint)
        {

        }
    }
}
