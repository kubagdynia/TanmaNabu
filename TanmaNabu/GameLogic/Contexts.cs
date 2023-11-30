using Entitas;
using System;
using TanmaNabu.Core;
using TanmaNabu.Core.Map;
using TanmaNabu.GameLogic.Game;

namespace TanmaNabu.GameLogic
{
    public partial class Contexts : IContexts
    {
        public static Contexts SharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = new Contexts();
                }

                return _sharedInstance;
            }
            set => _sharedInstance = value;
        }

        private static Contexts _sharedInstance;

        public GameTime GameTime { get; set; }

        public Map GameMap { get; set; }

        public GameContext Game { get; set; }

        public IContext[] AllContexts => new IContext[] { Game };

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
            IContext[] contexts = AllContexts;
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i].Reset();
            }
        }
    }
}
