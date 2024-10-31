using TanmaNabu.Core.Exceptions;

namespace TanmaNabu.Core.Map.Exceptions;

public class MapException(string message, string hint) : BaseCoreException(message, hint);