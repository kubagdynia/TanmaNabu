using System;
using System.Linq;
using Entitas;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TanmaNabu.Core;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Managers;
using TanmaNabu.GameLogic;
using TanmaNabu.GameLogic.Game;
using TanmaNabu.GameLogic.Systems;
using TanmaNabu.Core.Settings;

namespace TanmaNabu.States
{
    public class Game : BaseGame
    {
        protected Contexts Contexts;
        protected Systems Systems;
        protected Camera Camera;

        public Game()
            : base(new Vector2u(1440, 810), "Tanma Nabu", Color.Black, 60, false, true) // window
            //: base(new Vector2u(1920, 1080), "Tanma Nabu", Color.Black, 60, true, true) // full screen
        {

        }

        protected override void LoadContent()
        {
            GameSettings.Load();

            AssetManager.Instance.Map.Load("jungleMap", AssetManager.Instance.GetMapPath("jungle_map.tmx"));
        }

        protected override void Initialize(RenderTarget target)
        {
            Contexts = Contexts.SharedInstance;

            Contexts.GameMap.Load("jungleMap");

            Systems = CreateSystems(Contexts);

            // Call once on start
            Systems.Initialize();

            Camera = new Camera(target, Contexts);
        }

        protected override void Update(float deltaTime)
        {
            Contexts.Game.DeltaTime = deltaTime;
            Contexts.GameMap.Update(deltaTime);

            // Call every frame
            Systems.Execute();
            Systems.Cleanup();
        }

        protected override void Render(RenderTarget target, float deltaTime, GameTime gameTime)
        {
            var players = Contexts.Game.GetGroup(GameMatcher.Player);
            var entity = players.GetSingleEntity();

            Camera.Update(deltaTime, gameTime, entity.Position.X, entity.Position.Y);

            target.Draw(Contexts.GameMap.GetBackgroundTileMap());

            var entities = Contexts.Game.GetEntities(GameMatcher.Animation);
            foreach (var objEntity in entities.OrderBy(c => c.Position.Y))
            {
                target.Draw(objEntity.Animation.GetSprite());
            }

            target.Draw(Contexts.GameMap.GetForegroundTileMap());

#if DEBUG
            DrawCollisions(target);
#endif
        }

        private void DrawCollisions(RenderTarget target)
        {
            foreach (var item in Contexts.GameMap.MapData.CollidersLayer.Colliders)
            {
                var colRectangle = new RectangleShape(new Vector2f(item.Width, item.Height))
                {
                    Position = new Vector2f(item.Left, item.Top),
                    OutlineColor = new Color(255, 0, 0, 200),
                    OutlineThickness = 2,
                    FillColor = new Color(255, 0, 0, 50)
                };

                target.Draw(colRectangle);
            }
        }

        protected override void KeyPressed(object sender, KeyEventArgs e)
        {
            base.KeyPressed(sender, e);
        }

        protected override void KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        protected override void JoystickButtonPressed(object sender, JoystickButtonEventArgs arg)
        {

        }

        protected override void JoystickButtonReleased(object sender, JoystickButtonEventArgs arg)
        {

        }

        protected override void JoystickConnected(object sender, JoystickConnectEventArgs arg)
        {

        }

        protected override void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
        {

        }

        protected override void JoystickMoved(object sender, JoystickMoveEventArgs arg)
        {

        }

        protected override void Quit()
        {
            Systems.TearDown();
            GameSettings.CleanUp();

#if DEBUG
            "Quit Game :(".Log();
#endif
        }

        protected override void Resize(uint width, uint height)
        {
            
        }

        private Systems CreateSystems(Contexts contexts)
        {
            return new Systems()
                .Add(new InputSystem(contexts))
                .Add(new DebugMessageSystem(contexts))
                .Add(new SpawnSystem(contexts))
                .Add(new PositionSystem(contexts))
                .Add(new AnimationTypeSystem(contexts))
                .Add(new AnimationSystem(contexts));
        }
    }
}
