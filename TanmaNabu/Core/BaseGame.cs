using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace TanmaNabu.Core
{
    public abstract class BaseGame
    {
        // The limit of how many times we can update if lagging
        private const float UpdateLimit = 10;

        private readonly float _updateRate;
        private readonly Color _clearColor;

        protected readonly RenderWindow Window;

        private Time Time { get; set; }

        protected BaseGame(Vector2u windowSize, string windowTitle, Color clearColor, uint framerateLimit = 60,
           bool fullScreen = false, bool vsync = false)
        {
            _clearColor = clearColor;

            // The frequency at which our step will execute
            _updateRate = 1.0f / framerateLimit;

            if (fullScreen)
            {
                Window = new RenderWindow(new VideoMode(windowSize.X, windowSize.Y, 32), windowTitle, Styles.Fullscreen);
            }
            else
            {
                Window = new RenderWindow(new VideoMode(windowSize.X, windowSize.Y, 32), windowTitle, Styles.Default);

            }

            if (vsync)
            {
                Window.SetVerticalSyncEnabled(true);

            }
            else
            {
                Window.SetFramerateLimit(framerateLimit);
            }

            // Set up events
            Window.Closed += (sender, arg) => Window.Close();
            Window.Resized += (sender, arg) => Resize(arg.Width, arg.Height);

            // Key
            Window.KeyPressed += KeyPressed;
            Window.KeyReleased += KeyReleased;

            // Controller
            Window.JoystickConnected += JoystickConnected;
            Window.JoystickDisconnected += JoystickDisconnected;
            Window.JoystickButtonPressed += JoystickButtonPressed;
            Window.JoystickButtonReleased += JoystickButtonReleased;
            Window.JoystickMoved += JoystickMoved;
        }

        public void Run()
        {
            LoadContent();
            Initialize();

            var clock = new Clock();

            var totalTime = 0.0f;

            // Main game loop
            while (Window.IsOpen)
            {
                Time = clock.Restart();
                var deltaTime = Time.AsSeconds();

                if (deltaTime > 1)
                {
                    deltaTime = 0;
                }

                totalTime += deltaTime;
                var updateCount = 0;

                // While the total amount of time spend on the render step is
                // greater or equal to the update rate (1/x, in this game x = 60) and we have
                // not executed the update step 10 times then do the loop
                // If the counter hits 10 we break because it means that the
                // render step is lagging behind the update step
                while (totalTime >= _updateRate && updateCount < UpdateLimit)
                {
                    Window.DispatchEvents();

                    Joystick.Update();

                    Update(_updateRate);

                    // Subtract the update frequency from the total time
                    totalTime -= _updateRate;
                    // Increase the counter
                    updateCount++;
                }

                // clear the window with clear color
                Window.Clear(_clearColor);

                Render(totalTime / _updateRate, Time);

                Window.Display();
            }

            Quit();
        }

        protected abstract void LoadContent();

        protected abstract void Initialize();

        protected abstract void Update(float deltaTime);

        protected abstract void Render(float deltaTime, Time elapsedTime);

        protected abstract void Quit();

        protected abstract void Resize(uint width, uint height);

        protected virtual void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Q)
            {
                Window.Close();
            }
        }

        protected abstract void KeyReleased(object sender, KeyEventArgs e);

        protected abstract void JoystickConnected(object sender, JoystickConnectEventArgs arg);

        protected abstract void JoystickDisconnected(object sender, JoystickConnectEventArgs arg);

        protected abstract void JoystickButtonReleased(object sender, JoystickButtonEventArgs arg);

        protected abstract void JoystickButtonPressed(object sender, JoystickButtonEventArgs arg);

        protected abstract void JoystickMoved(object sender, JoystickMoveEventArgs arg);

        protected float GetFps()
        {
            return (1000000.0f / Time.AsMicroseconds());
        }
    }
}
