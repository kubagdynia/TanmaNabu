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
using TanmaNabu.Settings;

namespace TanmaNabu.States
{
    public class Game : BaseGame
    {
        protected Contexts Contexts;
        protected Systems Systems;

        public Game()
            : base(new Vector2u(1440, 810), "Tanma Nabu", Color.Black, 60, false, true)
        {

        }

        protected override void LoadContent()
        {
            GameSettings.Load();

            AssetManager.Instance.Map.Load("jungleMap", AssetManager.Instance.GetMapPath("jungle_map.tmx"));
        }

        protected override void Initialize()
        {
            Contexts = Contexts.SharedInstance;

            Contexts.GameMap.Load("jungleMap");

            Systems = CreateSystems(Contexts);

            // Call once on start
            Systems.Initialize();
        }

        protected override void Update(float deltaTime)
        {
            Contexts.Game.DeltaTime = deltaTime;
            Contexts.GameMap.Update(deltaTime);

            // Call every frame
            Systems.Execute();
            Systems.Cleanup();
        }

        protected override void Render(float deltaTime)
        {
            var players = Contexts.Game.GetGroup(GameMatcher.Player);
            var entity = players.GetSingleEntity();

            /* Hack to get rid of the annoying horizontal and vertical
               lines that are caused by floating point rendering */
            Vector2f correction = new Vector2f(
                ((int)(Window.DefaultView.Size.X) % 2 == 0 ? 0 : 1) * 0.5f,
                ((int)(Window.DefaultView.Size.Y) % 2 == 0 ? 0 : 1) * 0.5f);

            Contexts.GameMap.SetWorldView(Window, new Vector2f(entity.Position.X + correction.X, entity.Position.Y + correction.Y));

            Window.Draw(Contexts.GameMap.GetBackgroundTileMap());

            var entities = Contexts.Game.GetEntities(GameMatcher.Animation);
            foreach (var objEntity in entities.OrderBy(c => c.Position.Y))
            {
                Window.Draw(objEntity.Animation.GetSprite());
            }

            Window.Draw(Contexts.GameMap.GetForegroundTileMap());

#if DEBUG
            DrawCollisions();
#endif
        }

        private void DrawCollisions()
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

                Window.Draw(colRectangle);
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
