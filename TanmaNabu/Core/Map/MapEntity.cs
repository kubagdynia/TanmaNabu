using TanmaNabu.Core.Animation;

namespace TanmaNabu.Core.Map
{
    public class MapEntity
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string TilesetName { get; set; }

        /// <summary>
        /// Its an item from custom properties, for example Skeleton, Ghost
        /// </summary>
        public string ObjectType { get; set; }

        public bool Visible { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public AnimationType InitialState { get; set; }

        public int MovementSpeed { get; set; }

        public bool IsPlayer { get; set; }
    }
}
