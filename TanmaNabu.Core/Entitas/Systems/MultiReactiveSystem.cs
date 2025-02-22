﻿using System.Collections.Generic;

namespace Entitas;

/// A ReactiveSystem calls Execute(entities) if there were changes based on
/// the specified Collector and will only pass in changed entities.
/// A common use-case is to react to changes, e.g. a change of the position
/// of an entity to update the gameObject.transform.position
/// of the related gameObject.
public abstract class MultiReactiveSystem<TEntity, TContexts> : IReactiveSystem
    where TEntity : class, IEntity
    where TContexts : class, IContexts
{
    private readonly ICollector[] _collectors;
    private readonly HashSet<TEntity> _collectedEntities;
    private readonly List<TEntity> _buffer;
    private string _toStringCache;

    protected MultiReactiveSystem(TContexts contexts)
    {
        _collectors = GetTrigger(contexts);
        _collectedEntities = [];
        _buffer = [];
    }

    protected MultiReactiveSystem(ICollector[] collectors)
    {
        _collectors = collectors;
        _collectedEntities = [];
        _buffer = [];
    }

    /// Specify the collector that will trigger the ReactiveSystem.
    protected abstract ICollector[] GetTrigger(TContexts contexts);

    /// This will exclude all entities which don't pass the filter.
    protected abstract bool Filter(TEntity entity);

    protected abstract void Execute(List<TEntity> entities);

    /// Activates the ReactiveSystem and starts observing changes
    /// based on the specified Collector.
    /// ReactiveSystem are activated by default.
    public void Activate()
    {
        for (var i = 0; i < _collectors.Length; i++)
        {
            _collectors[i].Activate();
        }
    }

    /// Deactivates the ReactiveSystem.
    /// No changes will be tracked while deactivated.
    /// This will also clear the ReactiveSystem.
    /// ReactiveSystem are activated by default.
    public void Deactivate()
    {
        for (var i = 0; i < _collectors.Length; i++)
        {
            _collectors[i].Deactivate();
        }
    }

    /// Clears all accumulated changes.
    public void Clear()
    {
        for (var i = 0; i < _collectors.Length; i++)
        {
            _collectors[i].ClearCollectedEntities();
        }
    }

    /// Will call Execute(entities) with changed entities
    /// if there are any. Otherwise it will not call Execute(entities).
    public void Execute()
    {
        foreach (var collector in _collectors)
        {
            if (collector.Count != 0)
            {
                _collectedEntities.UnionWith(collector.GetCollectedEntities<TEntity>());
                collector.ClearCollectedEntities();
            }
        }

        foreach (var entity in _collectedEntities)
        {
            if (Filter(entity))
            {
                entity.Retain(this);
                _buffer.Add(entity);
            }
        }

        if (_buffer.Count != 0)
        {
            try
            {
                Execute(_buffer);
            }
            finally
            {
                for (var i = 0; i < _buffer.Count; i++)
                {
                    _buffer[i].Release(this);
                }
                _collectedEntities.Clear();
                _buffer.Clear();
            }
        }
    }

    public override string ToString()
    {
        if (_toStringCache == null)
        {
            _toStringCache = $"MultiReactiveSystem({GetType().Name})";
        }

        return _toStringCache;
    }

    ~MultiReactiveSystem()
    {
        Deactivate();
    }
}