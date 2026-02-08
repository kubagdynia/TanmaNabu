using SFML.Graphics;
using System.Collections.Generic;
using TiledSharp;

namespace TanmaNabu.Core.Map;

public interface ITileMap
{
    void Draw(RenderTexture target, RenderStates states);
    
    void Load(MapData data, IList<TmxLayer> layers);

    void Update(float deltaTime);
}