using System;
using System.Collections.Generic;

namespace Entitas;

public class ObjectPool<T>(Func<T> factoryMethod, Action<T> resetMethod = null)
{
    private readonly Stack<T> _objectPool = new();

    public T Get() => _objectPool.Count == 0 ? factoryMethod() : _objectPool.Pop();

    public void Push(T obj)
    {
        resetMethod?.Invoke(obj);
        _objectPool.Push(obj);
    }
}