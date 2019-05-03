using Entitas;

namespace TanmaNabu.GameLogic.Components
{
    public sealed class CharacterComponent : IComponent
    {
        public ObjectType ObjectType;

        public override string ToString()
        {
            return $"Character(ObjectType: {ObjectType}";
        }
    }
}
