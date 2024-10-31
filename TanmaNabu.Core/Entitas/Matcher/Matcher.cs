namespace Entitas;

public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity
{
    public int[] Indices => _indices ??= MergeIndices(_allOfIndexes, _anyOfIndexes, _noneOfIndexes);

    public int[] AllOfIndexes => _allOfIndexes;
    public int[] AnyOfIndexes => _anyOfIndexes;
    public int[] NoneOfIndexes => _noneOfIndexes;

    public string[] ComponentNames { get; set; }

    private int[] _indices;
    private int[] _allOfIndexes;
    private int[] _anyOfIndexes;
    private int[] _noneOfIndexes;

    IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indices)
    {
        _anyOfIndexes = DistinctIndices(indices);
        _indices = null;
        _isHashCached = false;
        return this;
    }

    IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers)
        => ((IAllOfMatcher<TEntity>)this).AnyOf(MergeIndices(matchers));

    public INoneOfMatcher<TEntity> NoneOf(params int[] indices)
    {
        _noneOfIndexes = DistinctIndices(indices);
        _indices = null;
        _isHashCached = false;
        return this;
    }

    public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers)
    {
        return NoneOf(MergeIndices(matchers));
    }

    public bool Matches(TEntity entity)
        => (_allOfIndexes == null || entity.HasComponents(_allOfIndexes))
           && (_anyOfIndexes == null || entity.HasAnyComponent(_anyOfIndexes))
           && (_noneOfIndexes == null || !entity.HasAnyComponent(_noneOfIndexes));
}