using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TanmaNabu.Core.Managers
{
    public class Manager<T> : IManager<T> where T : class
    {
        private static List<ManagerItem<T>> _items;

        private static List<ManagerItem<T>> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<ManagerItem<T>>();
                }
                return _items;
            }
        }

        public T LoadAndGet(string name, string filename) => LoadAndGet(name, filename, false);

        public T LoadAndGet(string name, string filename, bool overrideItem) => LoadAndGet(name, filename, overrideItem, null);

        public T LoadAndGet(string name, string filename, object parent) => LoadAndGet(name, filename, false, parent);

        public T LoadAndGet(string name, string filename, bool overrideItem, object parent)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(filename))
            {
                return null;
            }

            if (Exists(name, parent))
            {
                if (overrideItem)
                {
                    Remove(name, parent);
                }
                else
                {
                    return Get(name, parent);
                }
            }

            // Create instance of T class and send filename to its constructor
            var instance = Activator.CreateInstance(typeof(T), filename) as T;

            if (parent == null)
            {
                Items.Add(new ManagerItem<T>(name, instance));
                return instance;
            }

            Items.Add(new ManagerItem<T>(name, parent, instance));

            return instance;
        }

        public void Load(string name, string filename) => Load(name, filename, false);

        public void Load(string name, string filename, bool overrideItem) => Load(name, filename, overrideItem, null);

        public void Load(string name, string filename, object parent) => Load(name, filename, false, parent);

        public void Load(string name, string filename, bool overrideItem, object parent)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            if (Exists(name, parent))
            {
                if (overrideItem)
                {
                    Remove(name, parent);
                }
                else
                {
                    return;
                }
            }

            // Create instance of T class and send filename to its constructor
            var instance = Activator.CreateInstance(typeof(T), filename) as T;

            if (parent == null)
            {
                Items.Add(new ManagerItem<T>(name, instance));
                return;
            }

            Items.Add(new ManagerItem<T>(name, parent, instance));
        }


        public void Remove(string name) => Remove(name, null);

        public void Remove(string name, object parent)
        {
            if (parent == null)
            {
                Items.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                return;
            }

            Items.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent));
        }

        public void RemoveAll() => Items.Clear();

        public void RemoveParent(object parent)
        {
            Items.RemoveAll(c => c.Parent.Equals(parent));
        }

        public bool Exists(string name) => Exists(name, null);

        public bool Exists(string name, object parent)
        {
            if (parent == null)
            {
                return !string.IsNullOrWhiteSpace(name) && Items.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return !string.IsNullOrWhiteSpace(name) && Items.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent));
        }

        public T Get(string name) => Get(name, null);

        public T Get(string name, object parent)
        {
            if (!Exists(name, parent))
            {
                return default(T);
            }

            if (parent == null)
            {
                return Items.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Resource;
            }

            return Items.SingleOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent))?.Resource;
        }

        public Dictionary<object, IList<T>> GetGroupedByParent() => GetGroupedByParent(null);

        public Dictionary<object, IList<T>> GetGroupedByParent(string name)
        {
            var grouped = new Dictionary<object, IList<T>>();

            foreach (var item in Items.Where(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || name == null))
            {
                if (!grouped.ContainsKey(item.Parent))
                {
                    grouped.Add(item.Parent, new List<T> { item.Resource });
                    continue;
                }

                var groupedItem = grouped[item.Parent];
                groupedItem.Add(item.Resource);
            }

            return grouped;
        }
    }
}
