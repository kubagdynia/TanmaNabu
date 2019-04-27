using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TanmaNabu.Core;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Managers;
using TanmaNabu.Core.Map;
using TanmaNabu.Settings;

namespace TanmaNabu
{
    public class Game : BaseGame
    {
        private readonly Map _map;

        public Game()
            : base(new Vector2u(1440, 810), "Tanma Nabu", Color.Black, 60, false, true)
        {
            _map = new Map();
        }

        protected override void LoadContent()
        {
            GameSettings.Load();

            AssetManager.Instance.Map.Load("jungleMap", AssetManager.Instance.GetMapPath("jungle_map.tmx"));

            _map.Load("jungleMap");
        }

        protected override void Initialize()
        {

        }

        protected override void Update(float deltaTime)
        {

        }

        protected override void Render(float deltaTime)
        {
            // Draw green rectangle
            RectangleShape rectangle = new RectangleShape(new Vector2f(200, 200))
            {
                FillColor = Color.Green
            };

            // Set the rectangle in the center of the screen
            rectangle.Position = new Vector2f(
                (Window.Size.X / 2) - rectangle.Size.X / 2,
                (Window.Size.Y / 2) - rectangle.Size.Y / 2);

            Window.Draw(rectangle);
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
            GameSettings.CleanUp();

#if DEBUG
            "Quit Game :(".Log();
#endif
        }

        protected override void Resize(uint width, uint height)
        {

        }
    }
}
