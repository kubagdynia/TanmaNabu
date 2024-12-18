﻿using System;
using System.Collections.Generic;

namespace Entitas;

public sealed class PrimaryEntityIndex<TEntity, TKey> : AbstractEntityIndex<TEntity, TKey> where TEntity : class, IEntity
{
    private readonly Dictionary<TKey, TEntity> _index;

    public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey)
        : base(name, group, getKey)
    {
        _index = new Dictionary<TKey, TEntity>();
        Activate();
    }

    public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys)
        : base(name, group, getKeys)
    {
        _index = new Dictionary<TKey, TEntity>();
        Activate();
    }

    public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey, IEqualityComparer<TKey> comparer)
        : base(name, group, getKey)
    {
        _index = new Dictionary<TKey, TEntity>(comparer);
        Activate();
    }

    public PrimaryEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys, IEqualityComparer<TKey> comparer)
        : base(name, group, getKeys)
    {
        _index = new Dictionary<TKey, TEntity>(comparer);
        Activate();
    }

    public override void Activate()
    {
        base.Activate();
        IndexEntities(Group);
    }

    public TEntity GetEntity(TKey key)
    {
        _index.TryGetValue(key, out TEntity entity);
        return entity;
    }

    public override string ToString() => $"PrimaryEntityIndex({Name})";

    protected override void Clear()
    {
        foreach (var entity in _index.Values)
        {
            if (entity.Aerc is SafeAerc safeAerc)
            {
                if (safeAerc.Owners.Contains(this))
                {
                    entity.Release(this);
                }
            }
            else
            {
                entity.Release(this);
            }
        }

        _index.Clear();
    }

    protected override void AddEntity(TKey key, TEntity entity)
    {
        if (!_index.TryAdd(key, entity))
        {
            throw new EntityIndexException(
                $"Entity for key '{key}' already exists!",
                "Only one entity for a primary key is allowed.");
        }

        if (entity.Aerc is SafeAerc safeAerc)
        {
            if (!safeAerc.Owners.Contains(this))
            {
                entity.Retain(this);
            }
        }
        else
        {
            entity.Retain(this);
        }
    }

    protected override void RemoveEntity(TKey key, TEntity entity)
    {
        _index.Remove(key);

        if (entity.Aerc is SafeAerc safeAerc)
        {
            if (safeAerc.Owners.Contains(this))
            {
                entity.Release(this);
            }
        }
        else
        {
            entity.Release(this);
        }
    }
}