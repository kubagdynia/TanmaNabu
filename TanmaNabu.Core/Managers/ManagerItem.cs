namespace TanmaNabu.Core.Managers;

public class ManagerItem<T>(string name, object parent, T resource)
{
    /// <summary>
    /// Item name
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Parent can be used to group items into groups.
    /// In this way you will be able to get or delete items only from one group.
    /// </summary>
    public object Parent { get; } = parent;

    public T Resource { get; } = resource;

    public ManagerItem(string name, T resource)
        : this(name, string.Empty, resource)
    {
    }
}