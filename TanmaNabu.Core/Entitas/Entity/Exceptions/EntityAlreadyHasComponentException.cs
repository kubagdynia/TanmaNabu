namespace Entitas;

public class EntityAlreadyHasComponentException(int index, string message, string hint)
    : BaseEntitasException($"{message}\nEntity already has a component at index {index}!", hint);