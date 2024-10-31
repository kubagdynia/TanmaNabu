using System;
using System.Collections.Generic;

namespace Entitas;

/// Automatic Entity Reference Counting (AERC)
/// is used internally to prevent pooling retained entities.
/// If you use retain manually you also have to
/// release it manually at some point.
/// SafeAERC checks if the entity has already been
/// retained or released. It's slower, but you keep the information
/// about the owners.
public sealed class SafeAerc(IEntity entity) : IAerc
{
    public static readonly Func<IEntity, IAerc> Delegate = entity => new SafeAerc(entity);
        
    public int RetainCount => _owners.Count;
        
    public HashSet<object> Owners => _owners;

    private readonly HashSet<object> _owners = [];

    public void Retain(object owner)
    {
        if (!Owners.Add(owner))
        {
            throw new EntityIsAlreadyRetainedByOwnerException(entity, owner);
        }
    }

    public void Release(object owner)
    {
        if (!Owners.Remove(owner))
        {
            throw new EntityIsNotRetainedByOwnerException(entity, owner);
        }
    }
}