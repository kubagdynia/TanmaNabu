using SFML.Graphics;
using TanmaNabu.Core.Animation;

namespace TanmaNabu.GameLogic.Components;

public class AnimationFrame(int id, int duration, AnimationType animationType = AnimationType.None, IntRect rect = new())
{
    /// <summary>
    /// Tile id
    /// </summary>
    public int Id { get; set; } = id;

    public int Duration { get; set; } = duration;

    public IntRect Rect { get; set; } = rect;

    public AnimationType AnimationType { get; set; } = animationType;
}