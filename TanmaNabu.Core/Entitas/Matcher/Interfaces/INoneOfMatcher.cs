namespace Entitas;

public interface INoneOfMatcher<in TEntity> : ICompoundMatcher<TEntity> where TEntity : class, IEntity;