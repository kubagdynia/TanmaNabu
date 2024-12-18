﻿namespace Entitas;

public static class CollectorContextExtension
{
    /// Creates a Collector.
    public static ICollector<TEntity> CreateCollector<TEntity>(
        this IContext<TEntity> context, IMatcher<TEntity> matcher) where TEntity : class, IEntity
    {
        return context.CreateCollector(new TriggerOnEvent<TEntity>(matcher, GroupEvent.Added));
    }

    /// Creates a Collector.
    public static ICollector<TEntity> CreateCollector<TEntity>(
        this IContext<TEntity> context, IMatcher<TEntity> matcher, GroupEvent groupEvent) where TEntity : class, IEntity
    {
        return context.CreateCollector(new TriggerOnEvent<TEntity>(matcher, groupEvent));
    }

    /// Creates a Collector.
    public static ICollector<TEntity> CreateCollector<TEntity>(
        this IContext<TEntity> context, params TriggerOnEvent<TEntity>[] triggers) where TEntity : class, IEntity
    {
        var groups = new IGroup<TEntity>[triggers.Length];
        var groupEvents = new GroupEvent[triggers.Length];

        for (var i = 0; i < triggers.Length; i++)
        {
            groups[i] = context.GetGroup(triggers[i].Matcher);
            groupEvents[i] = triggers[i].GroupEvent;
        }

        return new Collector<TEntity>(groups, groupEvents);
    }
}