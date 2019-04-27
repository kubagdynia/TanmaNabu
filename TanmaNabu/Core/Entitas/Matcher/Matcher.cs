namespace Entitas
{
    public partial class Matcher<TEntity> : IAllOfMatcher<TEntity> where TEntity : class, IEntity
    {
        public int[] Indices
        {
            get
            {
                if (_indices == null)
                {
                    _indices = MergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices);
                }
                return _indices;
            }
        }

        public int[] AllOfIndices => _allOfIndices;
        public int[] AnyOfIndices => _anyOfIndices;
        public int[] NoneOfIndices => _noneOfIndices;

        public string[] componentNames { get; set; }

        private int[] _indices;
        private int[] _allOfIndices;
        private int[] _anyOfIndices;
        private int[] _noneOfIndices;

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params int[] indices)
        {
            _anyOfIndices = DistinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        IAnyOfMatcher<TEntity> IAllOfMatcher<TEntity>.AnyOf(params IMatcher<TEntity>[] matchers)
        {
            return ((IAllOfMatcher<TEntity>)this).AnyOf(MergeIndices(matchers));
        }

        public INoneOfMatcher<TEntity> NoneOf(params int[] indices)
        {
            _noneOfIndices = DistinctIndices(indices);
            _indices = null;
            _isHashCached = false;
            return this;
        }

        public INoneOfMatcher<TEntity> NoneOf(params IMatcher<TEntity>[] matchers)
        {
            return NoneOf(MergeIndices(matchers));
        }

        public bool Matches(TEntity entity)
        {
            return (_allOfIndices == null || entity.HasComponents(_allOfIndices))
                   && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
                   && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
        }
    }
}