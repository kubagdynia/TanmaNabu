namespace TanmaNabu.Core.Managers
{
    public class ManagerItem<T>
    {
        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parent can be used to group items into groups.
        /// In this way you will be able to get or delete items only from one group.
        /// </summary>
        public object Parent { get; }

        public T Resource { get; }

        public ManagerItem(string name, T resource)
            : this(name, string.Empty, resource)
        {
        }

        public ManagerItem(string name, object parent, T resource)
        {
            Name = name;
            Parent = parent;
            Resource = resource;
        }
    }
}
