using Entitas;

namespace TanmaNabu.GameLogic.Components;

public sealed class CharacterComponent : IComponent
{
    public ObjectType ObjectType;

    public override string ToString() => $"Character(ObjectType: {ObjectType}";
}