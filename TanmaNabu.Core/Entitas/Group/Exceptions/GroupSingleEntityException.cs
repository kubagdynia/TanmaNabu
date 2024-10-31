using System.Linq;

namespace Entitas;

public class GroupSingleEntityException<TEntity>(IGroup<TEntity> group) : BaseEntitasException(
    $"Cannot get the single entity from {group}!\nGroup contains {group.Count} entities:",
    string.Join("\n", group.GetEntities().Select(e => e.ToString()).ToArray()))
    where TEntity : class, IEntity;