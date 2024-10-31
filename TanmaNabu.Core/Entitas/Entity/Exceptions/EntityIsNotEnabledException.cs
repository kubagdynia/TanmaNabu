namespace Entitas;

public class EntityIsNotEnabledException(string message) : BaseEntitasException($"{message}\nEntity is not enabled!",
    "The entity has already been destroyed. You cannot modify destroyed entities.");