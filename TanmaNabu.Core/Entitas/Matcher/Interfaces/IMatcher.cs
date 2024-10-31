namespace Entitas;

public interface IMatcher<in TEntity> where TEntity : class, IEntity
{
    int[] Indices { get; }
    bool Matches(TEntity entity);
}