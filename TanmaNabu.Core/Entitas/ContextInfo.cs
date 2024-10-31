using System;

namespace Entitas;

public class ContextInfo(string name, string[] componentNames, Type[] componentTypes)
{
    public readonly string Name = name;
    public readonly string[] ComponentNames = componentNames;
    public readonly Type[] ComponentTypes = componentTypes;
}