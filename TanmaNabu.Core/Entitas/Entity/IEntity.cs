﻿using System;
using System.Collections.Generic;

namespace Entitas;

public delegate void EntityComponentChanged(
    IEntity entity, int index, IComponent component
);

public delegate void EntityComponentReplaced(
    IEntity entity, int index, IComponent previousComponent, IComponent newComponent
);

public delegate void EntityEvent(IEntity entity);

public interface IEntity : IAerc
{
    event EntityComponentChanged OnComponentAdded;
    event EntityComponentChanged OnComponentRemoved;
    event EntityComponentReplaced OnComponentReplaced;
    event EntityEvent OnEntityReleased;
    event EntityEvent OnDestroyEntity;

    int TotalComponents { get; }
    int CreationIndex { get; }
    bool IsEnabled { get; }

    Stack<IComponent>[] ComponentPools { get; }
    ContextInfo ContextInfo { get; }
    IAerc Aerc { get; }

    void Initialize(int creationIndex,
        int totalComponents,
        Stack<IComponent>[] componentPools,
        ContextInfo contextInfo = null,
        IAerc aerc = null);

    void Reactivate(int creationIndex);

    void AddComponent(int index, IComponent component);
    void RemoveComponent(int index);
    void ReplaceComponent(int index, IComponent component);

    IComponent GetComponent(int index);
    IComponent[] GetComponents();
    int[] GetComponentIndices();

    bool HasComponent(int index);
    bool HasComponents(int[] indices);
    bool HasAnyComponent(int[] indices);

    void RemoveAllComponents();

    Stack<IComponent> GetComponentPool(int index);
    IComponent CreateComponent(int index, Type type);
    T CreateComponent<T>(int index) where T : new();

    void Destroy();
    void InternalDestroy();
    void RemoveAllOnEntityReleasedHandlers();
}