namespace Entitas;

public interface ICompoundMatcher<in TEntity> : IMatcher<TEntity> where TEntity : class, IEntity
{
    int[] AllOfIndexes { get; }
    int[] AnyOfIndexes { get; }
    int[] NoneOfIndexes { get; }
}