using System.Collections.Generic;

namespace TanmaNabu.Core.Managers
{
    public interface IManager<T>
    {
        void Load(string name, string filename);

        void Load(string name, string filename, bool overrideItem);

        void Load(string name, string filename, object parent);

        void Load(string name, string filename, bool overrideItem, object parent);

        T LoadAndGet(string name, string filename);

        T LoadAndGet(string name, string filename, bool overrideItem);

        T LoadAndGet(string name, string filename, object parent);

        T LoadAndGet(string name, string filename, bool overrideItem, object parent);

        T Get(string name);

        T Get(string name, object parent);

        Dictionary<object, IList<T>> GetGroupedByParent();

        Dictionary<object, IList<T>> GetGroupedByParent(string name);

        void Remove(string name);

        void Remove(string name, object parent);

        void RemoveAll();

        void RemoveParent(object parent);

        bool Exists(string name);

        bool Exists(string name, object parent);
    }
}
