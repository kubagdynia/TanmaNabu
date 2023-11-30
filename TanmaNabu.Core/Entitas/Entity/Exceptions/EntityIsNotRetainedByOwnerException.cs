namespace Entitas
{
    public class EntityIsNotRetainedByOwnerException : BaseEntitasException
    {
        public EntityIsNotRetainedByOwnerException(IEntity entity, object owner)
            : base($"'{owner}' cannot release {entity}!\nEntity is not retained by this object!",
                "An entity can only be released from objects that retain it.")
        {
        }
    }
}