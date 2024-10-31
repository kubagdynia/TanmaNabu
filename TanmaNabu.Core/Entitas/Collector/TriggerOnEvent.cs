namespace Entitas;

public struct TriggerOnEvent<TEntity>(IMatcher<TEntity> matcher, GroupEvent groupEvent) where TEntity : class, IEntity
{
    public readonly IMatcher<TEntity> Matcher = matcher;
    public readonly GroupEvent GroupEvent = groupEvent;
}