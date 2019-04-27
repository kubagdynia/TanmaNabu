using System;
using System.Collections.Generic;

namespace Entitas
{
    public class ObjectPool<T>
    {
        private readonly Func<T> _factoryMethod;
        private readonly Action<T> _resetMethod;
        private readonly Stack<T> _objectPool;

        public ObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null)
        {
            _factoryMethod = factoryMethod;
            _resetMethod = resetMethod;
            _objectPool = new Stack<T>();
        }

        public T Get() => _objectPool.Count == 0 ? _factoryMethod() : _objectPool.Pop();

        public void Push(T obj)
        {
            _resetMethod?.Invoke(obj);
            _objectPool.Push(obj);
        }
    }
}
