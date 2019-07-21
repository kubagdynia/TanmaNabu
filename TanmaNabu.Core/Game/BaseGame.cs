﻿using SFML.Graphics;
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

        private RenderTexture _renderTexture;
        private Sprite _renderSprite;

        private GameTime _gameTime;

        protected BaseGame(Vector2u windowSize, string windowTitle, Color clearColor, uint framerateLimit = 60,
           bool fullScreen = false, bool vsync = false)
        {
            _clearColor = clearColor;

            // The frequency at which our step will execute
            _updateRate = 1.0f / framerateLimit;

            if (fullScreen)
            {
                Window = new RenderWindow(new VideoMode(windowSize.X, windowSize.Y, 32), windowTitle, Styles.Fullscreen);

                _renderTexture = new RenderTexture(windowSize.X, windowSize.Y);
                _renderSprite = new Sprite(_renderTexture.Texture, new IntRect(0, 0, (int)windowSize.X, (int)windowSize.Y));
            }
            else
            {
                Window = new RenderWindow(new VideoMode(windowSize.X, windowSize.Y, 32), windowTitle, Styles.Default);

                _renderTexture = new RenderTexture(windowSize.X, windowSize.Y);
                _renderSprite = new Sprite(_renderTexture.Texture, new IntRect(0, 0, (int)windowSize.X, (int)windowSize.Y));
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
            try
            {
                _gameTime = new GameTime();

                Initialize(_renderTexture, _gameTime);
                try
                {
                    var totalTime = 0.0f;

                    // Main game loop
                    while (Window.IsOpen)
                    {
                        _gameTime.Restart();

                        var deltaTime = _gameTime.ElapsedTime.AsSeconds();

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
                        _renderTexture.Clear(_clearColor);

                        // call render from the inheriting objects
                        Render(_renderTexture, totalTime / _updateRate, _gameTime);

                        _renderTexture.Display();

                        // draw it to the window
                        Window.Draw(_renderSprite);
                        Window.Display();
                    }
                }
                finally
                {
                    Deinitialize();
                }
            }
            finally
            {
                UnloadContent();
            }

            Quit();
        }

        protected abstract void LoadContent();

        protected abstract void UnloadContent();

        protected abstract void Initialize(RenderTarget target, GameTime gameTime);

        protected abstract void Deinitialize();

        protected abstract void Update(float deltaTime);

        protected abstract void Render(RenderTarget target, float deltaTime, GameTime gameTime);

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
            return (1000000.0f / _gameTime.ElapsedTime.AsMicroseconds());
        }
    }
}
