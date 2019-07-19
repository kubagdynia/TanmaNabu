using System.Collections.Generic;

namespace Entitas
{
    /// Automatic Entity Reference Counting (AERC)
    /// is used internally to prevent pooling retained entities.
    /// If you use retain manually you also have to
    /// release it manually at some point.
    /// SafeAERC checks if the entity has already been
    /// retained or released. It's slower, but you keep the information
    /// about the owners.
    public sealed class SafeAerc : IAerc
    {
        private readonly IEntity _entity;

        public int RetainCount => Owners.Count;

        public HashSet<object> Owners { get; } = new HashSet<object>();

        public SafeAerc(IEntity entity) => _entity = entity;

        public void Retain(object owner)
        {
            if (!Owners.Add(owner))
            {
                throw new EntityIsAlreadyRetainedByOwnerException(_entity, owner);
            }
        }

        public void Release(object owner)
        {
            if (!Owners.Remove(owner))
            {
                throw new EntityIsNotRetainedByOwnerException(_entity, owner);
            }
        }
    }
}
