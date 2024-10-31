using Entitas;
using System;
using TanmaNabu.Core.Game;
using TanmaNabu.Core.Map;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic;

public partial class Contexts : IContexts
{
    private static Contexts _sharedInstance;

    public static Contexts SharedInstance
    {
        get { return _sharedInstance ??= new Contexts(); }
        set => _sharedInstance = value;
    }

    public GameTime GameTime { get; set; }

    public Map GameMap { get; set; }

    public GameContext Game { get; set; }

    public IContext[] AllContexts => [Game];

    public Contexts()
    {
        GameMap = new Map();

        Game = new GameContext();

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => Attribute.IsDefined(method, typeof(PostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors)
        {
            postConstructor.Invoke(this, null);
        }
    }

    public void Reset()
    {
        var contexts = AllContexts;
        for (var i = 0; i < contexts.Length; i++)
        {
            contexts[i].Reset();
        }
    }
}