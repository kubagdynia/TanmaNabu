using System.Collections.ObjectModel;

namespace TanmaNabu.Core.Map
{
    public class CollidersLayer : TiledObjectLayer
    {
        private Collection<DataStructures.IntRect> _colliders;

        public Collection<DataStructures.IntRect> Colliders =>
            _colliders ?? (_colliders = new Collection<DataStructures.IntRect>());
    }
}
