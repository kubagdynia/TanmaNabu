﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas;

/// A context manages the lifecycle of entities and groups.
/// You can create and destroy entities and get groups of entities.
/// The prefered way to create a context is to use the generated methods
/// from the code generator, e.g. var context = new GameContext();
public class Context<TEntity> : IContext<TEntity> where TEntity : class, IEntity
{
    /// Occurs when an entity gets created.
    public event ContextEntityChanged OnEntityCreated;

    /// Occurs when an entity will be destroyed.
    public event ContextEntityChanged OnEntityWillBeDestroyed;

    /// Occurs when an entity got destroyed.
    public event ContextEntityChanged OnEntityDestroyed;

    /// Occurs when a group gets created for the first time.
    public event ContextGroupChanged OnGroupCreated;

    /// The total amount of components an entity can possibly have.
    /// This value is generated by the code generator,
    /// e.g ComponentLookup.TotalComponents.
    public int TotalComponents => _totalComponents;

    /// Returns all componentPools. componentPools is used to reuse
    /// removed components.
    /// Removed components will be pushed to the componentPool.
    /// Use entity.CreateComponent(index, type) to get a new or reusable
    /// component from the componentPool.
    public Stack<IComponent>[] ComponentPools => _componentPools;

    /// The contextInfo contains information about the context.
    /// It's used to provide better error messages.
    public ContextInfo ContextInfo => _contextInfo;

    /// Returns the number of entities in the context.
    public int Count => _entities.Count;

    /// Returns the number of entities in the internal ObjectPool
    /// for entities which can be reused.
    public int ReusableEntitiesCount => _reusableEntities.Count;

    /// Returns the number of entities that are currently retained by
    /// other objects (e.g. Group, Collector, ReactiveSystem).
    public int RetainedEntitiesCount => _retainedEntities.Count;

    private readonly int _totalComponents;

    private readonly Stack<IComponent>[] _componentPools;
    private readonly ContextInfo _contextInfo;
    private readonly Func<IEntity, IAerc> _aercFactory;
    readonly Func<TEntity> _entityFactory;

    private readonly HashSet<TEntity> _entities = new(EntityEqualityComparer<TEntity>.Comparer);
    private readonly Stack<TEntity> _reusableEntities = new();
    private readonly HashSet<TEntity> _retainedEntities = new(EntityEqualityComparer<TEntity>.Comparer);

    private readonly Dictionary<IMatcher<TEntity>, IGroup<TEntity>> _groups = new();
    private readonly List<IGroup<TEntity>>[] _groupsForIndex;
    private readonly ObjectPool<List<GroupChanged<TEntity>>> _groupChangedListPool;

    private readonly Dictionary<string, IEntityIndex> _entityIndices;

    private int _creationIndex;

    private TEntity[] _entitiesCache;

    // Cache delegates to avoid gc allocations
    private readonly EntityComponentChanged _cachedEntityChanged;
    private readonly EntityComponentReplaced _cachedComponentReplaced;
    private readonly EntityEvent _cachedEntityReleased;
    private readonly EntityEvent _cachedDestroyEntity;

    /// The prefered way to create a context is to use the generated methods
    /// from the code generator, e.g. var context = new GameContext();
    public Context(int totalComponents, Func<TEntity> entityFactory) : this(totalComponents, 0, null, null, entityFactory)
    {

    }

    /// The prefered way to create a context is to use the generated methods
    /// from the code generator, e.g. var context = new GameContext();
    public Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo, Func<IEntity, IAerc> aercFactory, Func<TEntity> entityFactory)
    {
        _totalComponents = totalComponents;
        _creationIndex = startCreationIndex;

        if (contextInfo != null)
        {
            _contextInfo = contextInfo;
            if (contextInfo.ComponentNames.Length != totalComponents)
            {
                throw new ContextInfoException(this, contextInfo);
            }
        }
        else
        {
            _contextInfo = CreateDefaultContextInfo();
        }
            
        _aercFactory = aercFactory ?? SafeAerc.Delegate;

        _aercFactory = aercFactory ?? (entity => new SafeAerc(entity));
        _entityFactory = entityFactory;

        _groupsForIndex = new List<IGroup<TEntity>>[totalComponents];
        _componentPools = new Stack<IComponent>[totalComponents];
        _entityIndices = new Dictionary<string, IEntityIndex>();
        _groupChangedListPool = new ObjectPool<List<GroupChanged<TEntity>>>(
            () => [],
            list => list.Clear()
        );

        // Cache delegates to avoid gc allocations
        _cachedEntityChanged = UpdateGroupsComponentAddedOrRemoved;
        _cachedComponentReplaced = UpdateGroupsComponentReplaced;
        _cachedEntityReleased = OnEntityReleased;
        _cachedDestroyEntity = OnDestroyEntity;
    }

    private ContextInfo CreateDefaultContextInfo()
    {
        var componentNames = new string[_totalComponents];
        for (var i = 0; i < componentNames.Length; i++)
        {
            componentNames[i] = "Index " + i;
        }

        return new ContextInfo("Unnamed Context", componentNames, null);
    }

    /// Creates a new entity or gets a reusable entity from the
    /// internal ObjectPool for entities.
    public TEntity CreateEntity()
    {
        TEntity entity;

        if (_reusableEntities.Count > 0)
        {
            entity = _reusableEntities.Pop();
            entity.Reactivate(_creationIndex++);
        }
        else
        {
            entity = _entityFactory();
            entity.Initialize(_creationIndex++, _totalComponents, _componentPools, _contextInfo, _aercFactory(entity));
        }

        _entities.Add(entity);
        entity.Retain(this);
        _entitiesCache = null;

        entity.OnComponentAdded += _cachedEntityChanged;
        entity.OnComponentRemoved += _cachedEntityChanged;
        entity.OnComponentReplaced += _cachedComponentReplaced;
        entity.OnEntityReleased += _cachedEntityReleased;
        entity.OnDestroyEntity += _cachedDestroyEntity;

        OnEntityCreated?.Invoke(this, entity);

        return entity;
    }

    /// Destroys all entities in the context.
    /// Throws an exception if there are still retained entities.
    public void DestroyAllEntities()
    {
        var entities = GetEntities();
        foreach (var entity in entities)
        {
            entity.Destroy();
        }

        _entities.Clear();

        if (_retainedEntities.Count != 0)
        {
            throw new ContextStillHasRetainedEntitiesException(this, _retainedEntities.ToArray());
        }
    }

    /// Determines whether the context has the specified entity.
    public bool HasEntity(TEntity entity)
        => _entities.Contains(entity);

    /// Returns all entities which are currently in the context.
    public TEntity[] GetEntities()
        =>  _entitiesCache ??= _entities.ToArray();
        
    /// Returns all entities matching the specified matcher.
    public TEntity[] GetEntities(IMatcher<TEntity> matcher)
        => GetGroup(matcher).GetEntities();
        

    /// Returns a group for the specified matcher.
    /// Calling context.GetGroup(matcher) with the same matcher will always
    /// return the same instance of the group.
    public IGroup<TEntity> GetGroup(IMatcher<TEntity> matcher)
    {
        if (_groups.TryGetValue(matcher, out var group)) return group;

        group = new Group<TEntity>(matcher);
        var entities = GetEntities();
        foreach (var entity in entities)
        {
            group.HandleEntitySilently(entity);
        }
        _groups.Add(matcher, group);

        foreach (var index in matcher.Indices)
        {
            _groupsForIndex[index] ??= new List<IGroup<TEntity>>();
            _groupsForIndex[index].Add(group);
        }

        OnGroupCreated?.Invoke(this, group);

        return group;
    }

    /// Adds the IEntityIndex for the specified name.
    /// There can only be one IEntityIndex per name.
    public void AddEntityIndex(IEntityIndex entityIndex)
    {
        if (!_entityIndices.TryAdd(entityIndex.Name, entityIndex))
        {
            throw new ContextEntityIndexDoesAlreadyExistException(this, entityIndex.Name);
        }
    }

    /// Gets the IEntityIndex for the specified name.
    public IEntityIndex GetEntityIndex(string name)
    {
        if (!_entityIndices.TryGetValue(name, out var entityIndex))
        {
            throw new ContextEntityIndexDoesNotExistException(this, name);
        }

        return entityIndex;
    }

    /// Resets the creationIndex back to 0.
    public void ResetCreationIndex() => _creationIndex = 0;

    /// Clears the componentPool at the specified index.
    public void ClearComponentPool(int index)
        => _componentPools[index]?.Clear();

    /// Clears all componentPools.
    public void ClearComponentPools()
    {
        for (var i = 0; i < _componentPools.Length; i++)
        {
            ClearComponentPool(i);
        }
    }

    /// Resets the context (destroys all entities and
    /// resets creationIndex back to 0).
    public void Reset()
    {
        DestroyAllEntities();
        ResetCreationIndex();
    }

    /// Removes all event handlers
    /// OnEntityCreated, OnEntityWillBeDestroyed,
    /// OnEntityDestroyed and OnGroupCreated
    public void RemoveAllEventHandlers()
    {
        OnEntityCreated = null;
        OnEntityWillBeDestroyed = null;
        OnEntityDestroyed = null;
        OnGroupCreated = null;
    }

    public override string ToString() => _contextInfo.Name;

    private void UpdateGroupsComponentAddedOrRemoved(IEntity entity, int index, IComponent component)
    {
        var groups = _groupsForIndex[index];
        if (groups == null) return;

        var events = _groupChangedListPool.Get();

        var tEntity = (TEntity)entity;

        foreach (var group in groups)
        {
            events.Add(group.HandleEntity(tEntity));
        }

        for (var i = 0; i < events.Count; i++)
        {
            var groupChangedEvent = events[i];
            groupChangedEvent?.Invoke(
                groups[i], tEntity, index, component
            );
        }

        _groupChangedListPool.Push(events);
    }

    private void UpdateGroupsComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
    {
        var groups = _groupsForIndex[index];
        if (groups == null) return;

        var tEntity = (TEntity)entity;

        foreach (var group in groups)
        {
            group.UpdateEntity(
                tEntity, index, previousComponent, newComponent
            );
        }
    }

    private void OnEntityReleased(IEntity entity)
    {
        if (entity.IsEnabled)
        {
            throw new EntityIsNotDestroyedException(
                $"Cannot release {entity}!"
            );
        }
        var tEntity = (TEntity)entity;
        entity.RemoveAllOnEntityReleasedHandlers();
        _retainedEntities.Remove(tEntity);
        _reusableEntities.Push(tEntity);
    }

    private void OnDestroyEntity(IEntity entity)
    {
        var tEntity = (TEntity)entity;

        var removed = _entities.Remove(tEntity);
        if (!removed)
        {
            throw new ContextDoesNotContainEntityException(
                $"'{this}' cannot destroy {tEntity}!", "This cannot happen!?!"
            );
        }
        _entitiesCache = null;

        OnEntityWillBeDestroyed?.Invoke(this, tEntity);

        tEntity.InternalDestroy();

        OnEntityDestroyed?.Invoke(this, tEntity);

        if (tEntity.RetainCount == 1)
        {
            // Can be released immediately without
            // adding to _retainedEntities
            tEntity.OnEntityReleased -= _cachedEntityReleased;
            _reusableEntities.Push(tEntity);
            tEntity.Release(this);
            tEntity.RemoveAllOnEntityReleasedHandlers();
        }
        else
        {
            _retainedEntities.Add(tEntity);
            tEntity.Release(this);
        }
    }
}