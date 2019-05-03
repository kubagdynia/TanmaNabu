using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TanmaNabu.Core.Animation;

namespace TanmaNabu.GameLogic.Components
{
    public class AnimationFrame
    {
        /// <summary>
        /// Tile id
        /// </summary>
        public int Id { get; set; }

        public int Duration { get; set; }

        public IntRect Rect { get; set; }

        public AnimationType AnimationType { get; set; }

        public AnimationFrame(int id, int duration)
        {
            Id = id;
            Duration = duration;
        }

        public AnimationFrame(int id, int duration, AnimationType animationType, IntRect rect)
        {
            Id = id;
            Duration = duration;
            AnimationType = animationType;
            Rect = rect;
        }
    }
}
