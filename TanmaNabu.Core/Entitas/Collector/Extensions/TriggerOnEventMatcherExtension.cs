﻿namespace Entitas;

public static class TriggerOnEventMatcherExtension
{
    public static TriggerOnEvent<TEntity> Added<TEntity>(this IMatcher<TEntity> matcher) where TEntity : class, IEntity
        => new(matcher, GroupEvent.Added);

    public static TriggerOnEvent<TEntity> Removed<TEntity>(this IMatcher<TEntity> matcher) where TEntity : class, IEntity
        => new(matcher, GroupEvent.Removed);

    public static TriggerOnEvent<TEntity> AddedOrRemoved<TEntity>(this IMatcher<TEntity> matcher) where TEntity : class, IEntity
        => new(matcher, GroupEvent.AddedOrRemoved);
}