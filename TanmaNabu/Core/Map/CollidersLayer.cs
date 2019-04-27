using System.Collections.ObjectModel;

namespace TanmaNabu.Core.Map
{
    public class CollidersLayer : TiledObjectLayer
    {
        private Collection<DataStructures.IntRect> _colliders;

        public Collection<DataStructures.IntRect> Colliders
        {
            get
            {
                if (_colliders == null)
                {
                    _colliders = new Collection<DataStructures.IntRect>();
                }
                return _colliders;
            }
            set => _colliders = value;
        }
    }
}
