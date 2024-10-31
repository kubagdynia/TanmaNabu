namespace Entitas;

public class EntityDoesNotHaveComponentException(int index, string message, string hint)
    : BaseEntitasException($"{message}\nEntity does not have a component at index {index}!", hint);