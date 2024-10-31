using System.Collections.Generic;

namespace Entitas;

/// Systems provide a convenient way to group systems.
/// You can add IInitializeSystem, IExecuteSystem, ICleanupSystem,
/// ITearDownSystem, ReactiveSystem and other nested Systems instances.
/// All systems will be initialized and executed based on the order
/// you added them.
public class Systems : IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
{
    protected readonly List<IInitializeSystem> InitializeSystems = [];
    protected readonly List<IExecuteSystem> ExecuteSystems = [];
    protected readonly List<ICleanupSystem> CleanupSystems = [];
    protected readonly List<ITearDownSystem> TearDownSystems = [];

    /// Adds the system instance to the systems list.
    public virtual Systems Add(ISystem system)
    {
        if (system is IInitializeSystem initializeSystem) InitializeSystems.Add(initializeSystem);
        if (system is IExecuteSystem executeSystem) ExecuteSystems.Add(executeSystem);
        if (system is ICleanupSystem cleanupSystem) CleanupSystems.Add(cleanupSystem);
        if (system is ITearDownSystem tearDownSystem) TearDownSystems.Add(tearDownSystem);
        return this;
    }
        
    public void Remove(ISystem system)
    {
        if (system is IInitializeSystem initializeSystem) InitializeSystems.Remove(initializeSystem);
        if (system is IExecuteSystem executeSystem) ExecuteSystems.Remove(executeSystem);
        if (system is ICleanupSystem cleanupSystem) CleanupSystems.Remove(cleanupSystem);
        if (system is ITearDownSystem tearDownSystem) TearDownSystems.Remove(tearDownSystem);
    }

    /// Calls Initialize() on all IInitializeSystem and other
    /// nested Systems instances in the order you added them.
    public virtual void Initialize()
    {
        for (var i = 0; i < InitializeSystems.Count; i++)
        {
            InitializeSystems[i].Initialize();
        }
    }

    /// Calls Execute() on all IExecuteSystem and other
    /// nested Systems instances in the order you added them.
    public virtual void Execute()
    {
        for (var i = 0; i < ExecuteSystems.Count; i++)
        {
            ExecuteSystems[i].Execute();
        }
    }

    /// Calls Cleanup() on all ICleanupSystem and other
    /// nested Systems instances in the order you added them.
    public virtual void Cleanup()
    {
        for (var i = 0; i < CleanupSystems.Count; i++)
        {
            CleanupSystems[i].Cleanup();
        }
    }

    /// Calls TearDown() on all ITearDownSystem  and other
    /// nested Systems instances in the order you added them.
    public virtual void TearDown()
    {
        for (var i = 0; i < TearDownSystems.Count; i++)
        {
            TearDownSystems[i].TearDown();
        }
    }

    /// Activates all ReactiveSystems in the systems list.
    public void ActivateReactiveSystems()
    {
        foreach (var system in ExecuteSystems)
        {
            if (system is IReactiveSystem reactiveSystem)
            {
                reactiveSystem.Activate();
            }

            if (system is Systems nestedSystems)
            {
                nestedSystems.ActivateReactiveSystems();
            }
        }
    }

    /// Deactivates all ReactiveSystems in the systems list.
    /// This will also clear all ReactiveSystems.
    /// This is useful when you want to soft-restart your application and
    /// want to reuse your existing system instances.
    public void DeactivateReactiveSystems()
    {
        foreach (var system in ExecuteSystems)
        {
            if (system is IReactiveSystem reactiveSystem)
            {
                reactiveSystem.Deactivate();
            }

            if (system is Systems nestedSystems)
            {
                nestedSystems.DeactivateReactiveSystems();
            }
        }
    }

    /// Clears all ReactiveSystems in the systems list.
    public void ClearReactiveSystems()
    {
        foreach (var system in ExecuteSystems)
        {
            if (system is IReactiveSystem reactiveSystem)
            {
                reactiveSystem.Clear();
            }

            if (system is Systems nestedSystems)
            {
                nestedSystems.ClearReactiveSystems();
            }
        }
    }
}