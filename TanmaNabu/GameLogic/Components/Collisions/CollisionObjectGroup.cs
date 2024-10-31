using System.Collections.Generic;

namespace TanmaNabu.GameLogic.Components;

public class CollisionObjectGroup
{
    public int TileId { get; set; }

    private List<CollisionObject> _collissions;

    public List<CollisionObject> Collissions => _collissions ??= [];

    /// <summary>
    /// Weight of the object
    /// </summary>
    public int Weight { get; set; }
}