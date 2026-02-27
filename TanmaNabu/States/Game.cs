using System.Collections.Generic;
using Entitas;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TanmaNabu.Core.Game;
using TanmaNabu.Core.Extensions;
using TanmaNabu.Core.Managers;
using TanmaNabu.GameLogic;
using TanmaNabu.GameLogic.Game;
using TanmaNabu.GameLogic.Systems;
using TanmaNabu.Core.Settings;
using IntRect = TanmaNabu.Core.DataStructures.IntRect;

namespace TanmaNabu.States;

public class Game : BaseGame
{
    private Contexts _contexts;
    private Systems _systems;
    private Camera _camera;

    // Reusable list for depth-sorting entities – avoids LINQ OrderBy allocation every frame
    private readonly List<GameEntity> _renderEntities = new();

    public Game()
    //    : base(new Vector2u(1440, 810), "Tanma Nabu", Color.Black, 60, false, false) // window
    : base(new Vector2u(1920, 1080), "Tanma Nabu", Color.Black, 60, true, false) // full screen
    {

    }

    protected override void LoadContent()
    {
#if DEBUG
        "Load content".Log();
#endif
        GameSettings.Load();

        AssetManager.Map.Load("jungleMap", AssetManager.Instance.GetMapPath("jungle_map.tmx"));
    }

    protected override void UnloadContent()
    {
#if DEBUG
        "Unload content".Log();
#endif
        AssetManager.CleanUp();
    }

    protected override void Initialize(RenderTexture target, GameTime gameTime)
    {
#if DEBUG
        "Initialize".Log();
#endif
        _contexts = Contexts.SharedInstance;

        _contexts.GameTime = gameTime;
        _contexts.GameMap.Load("jungleMap");

        _systems = CreateSystems(_contexts);

        // Call once on start
        _systems.Initialize();

        _camera = new Camera(target, _contexts);
    }

    protected override void Deinitialize()
    {
#if DEBUG
        "Deinitialize".Log();
#endif
        _systems.TearDown();
        GameSettings.CleanUp();
    }

    protected override void SampleInput()
    {
        var state = _contexts.Game.InputState;

        // Movement
        state.MoveX = 0f;
        state.MoveY = 0f;
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left)  || Keyboard.IsKeyPressed(Keyboard.Key.A)) state.MoveX -= 1f;
        if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D)) state.MoveX += 1f;
        if (Keyboard.IsKeyPressed(Keyboard.Key.Up)    || Keyboard.IsKeyPressed(Keyboard.Key.W)) state.MoveY -= 1f;
        if (Keyboard.IsKeyPressed(Keyboard.Key.Down)  || Keyboard.IsKeyPressed(Keyboard.Key.S)) state.MoveY += 1f;

        // Zoom
        state.ZoomReset = Keyboard.IsKeyPressed(Keyboard.Key.Home);
        state.ZoomDelta = 0f;
        if (!state.ZoomReset)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp))   state.ZoomDelta = -0.01f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown)) state.ZoomDelta =  0.01f;
        }
    }

    protected override void Update(float deltaTime)
    {
        _contexts.Game.DeltaTime = deltaTime;
        _contexts.GameMap.Update(deltaTime);

        _systems.Execute();
        _systems.Cleanup();

        // Camera updated in fixed-update, synchronised with player movement
        var entity = _contexts.Game.GetGroup(GameMatcher.Player).GetSingleEntity();
        _camera.Tick(deltaTime, entity.Position.X, entity.Position.Y);
    }

    protected override void Render(RenderTexture target, float deltaTime, GameTime gameTime)
    {
        // Apply camera view with alpha interpolation (0..1) between fixed-update steps
        _camera.Apply(deltaTime);

        _contexts.GameMap.GetBackgroundTileMap().Draw(target, RenderStates.Default);

        _renderEntities.Clear();
        _renderEntities.AddRange(_contexts.Game.GetEntities(GameMatcher.Animation));
        _renderEntities.Sort(static (a, b) => a.Position.Y.CompareTo(b.Position.Y));
        foreach (var objEntity in _renderEntities)
        {
            target.Draw(objEntity.Animation.GetSprite());
        }

        _contexts.GameMap.GetForegroundTileMap().Draw(target, RenderStates.Default);

#if DEBUG
        DrawCollisions(target);
#endif
    }

#if DEBUG
    // Reused across DrawCollisions calls – avoids per-collider allocation every frame
    private readonly RectangleShape _debugRect = new()
    {
        OutlineColor    = new Color(255, 0, 0, 200),
        OutlineThickness = 2,
        FillColor       = new Color(255, 0, 0, 50)
    };

    private void DrawCollisions(RenderTexture target)
    {
        foreach (IntRect item in _contexts.GameMap.MapData.CollidersLayer.Colliders)
        {
            _debugRect.Size     = new Vector2f(item.Width, item.Height);
            _debugRect.Position = new Vector2f(item.Left, item.Top);
            target.Draw(_debugRect);
        }
    }
#endif

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