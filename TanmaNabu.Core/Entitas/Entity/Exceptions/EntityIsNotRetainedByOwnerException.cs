namespace Entitas;

public class EntityIsNotRetainedByOwnerException(IEntity entity, object owner) : BaseEntitasException(
    $"'{owner}' cannot release {entity}!\nEntity is not retained by this object!",
    "An entity can only be released from objects that retain it.");