using TanmaNabu.Core.DataStructures;

namespace TanmaNabu.GameLogic.Components;

public class CollisionObject
{
    public int Id { get; set; }

    public IntRect CollisionRect { get; set; }
}