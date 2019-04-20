using System;

namespace Entitas
{
    public abstract class AbstractEntityIndex<TEntity, TKey> : IEntityIndex where TEntity : class, IEntity
    {
        public string Name { get; }

        protected readonly IGroup<TEntity> Group;
        protected readonly Func<TEntity, IComponent, TKey> GetKey;
        protected readonly Func<TEntity, IComponent, TKey[]> GetKeys;
        protected readonly bool IsSingleKey;

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey> getKey)
        {
            Name = name;
            Group = group;
            GetKey = getKey;
            IsSingleKey = true;
        }

        protected AbstractEntityIndex(string name, IGroup<TEntity> group, Func<TEntity, IComponent, TKey[]> getKeys)
        {
            Name = name;
            Group = group;
            GetKeys = getKeys;
            IsSingleKey = false;
        }

        public virtual void Activate()
        {
            Group.OnEntityAdded += OnEntityAdded;
            Group.OnEntityRemoved += OnEntityRemoved;
        }

        public virtual void Deactivate()
        {
            Group.OnEntityAdded -= OnEntityAdded;
            Group.OnEntityRemoved -= OnEntityRemoved;
            Clear();
        }

        public override string ToString()
        {
            return Name;
        }

        protected void IndexEntities(IGroup<TEntity> group)
        {
            foreach (var entity in group)
            {
                if (IsSingleKey)
                {
                    AddEntity(GetKey(entity, null), entity);
                }
                else
                {
                    var keys = GetKeys(entity, null);
                    foreach (var key in keys)
                    {
                        AddEntity(key, entity);
                    }
                }
            }
        }

        protected void OnEntityAdded(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (IsSingleKey)
            {
                AddEntity(GetKey(entity, component), entity);
            }
            else
            {
                var keys = GetKeys(entity, component);
                foreach (var key in keys)
                {
                    AddEntity(key, entity);
                }
            }
        }

        protected void OnEntityRemoved(IGroup<TEntity> group, TEntity entity, int index, IComponent component)
        {
            if (IsSingleKey)
            {
                RemoveEntity(GetKey(entity, component), entity);
            }
            else
            {
                var keys = GetKeys(entity, component);
                foreach (var key in keys)
                {
                    RemoveEntity(key, entity);
                }
            }
        }

        protected abstract void AddEntity(TKey key, TEntity entity);

        protected abstract void RemoveEntity(TKey key, TEntity entity);

        protected abstract void Clear();

        ~AbstractEntityIndex()
        {
            Deactivate();
        }
    }
}
