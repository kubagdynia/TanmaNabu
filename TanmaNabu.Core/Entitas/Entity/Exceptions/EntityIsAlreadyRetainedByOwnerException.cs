namespace Entitas;

public class EntityIsAlreadyRetainedByOwnerException(IEntity entity, object owner) : BaseEntitasException(
    $"'{owner}' cannot retain {entity}!\nEntity is already retained by this object!",
    "The entity must be released by this object first.");