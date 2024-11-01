using System.Linq;
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

namespace TanmaNabu.States;

public class Game : BaseGame
{
    private Contexts _contexts;
    private Systems _systems;
    private Camera _camera;

    public Game()
        : base(new Vector2u(1440, 810), "Tanma Nabu", Color.Black, 60, false, false) // window
    //: base(new Vector2u(1920, 1080), "Tanma Nabu", Color.Black, 60, true, false) // full screen
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

    protected override void Initialize(RenderTarget target, GameTime gameTime)
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

    protected override void Update(float deltaTime)
    {
        _contexts.Game.DeltaTime = deltaTime;
        _contexts.GameMap.Update(deltaTime);

        // Call every frame
        _systems.Execute();
        _systems.Cleanup();
    }

    protected override void Render(RenderTarget target, float deltaTime, GameTime gameTime)
    {
        var players = _contexts.Game.GetGroup(GameMatcher.Player);
        var entity = players.GetSingleEntity();

        _camera.Update(deltaTime, gameTime, entity.Position.X, entity.Position.Y);

        target.Draw(_contexts.GameMap.GetBackgroundTileMap());

        var entities = _contexts.Game.GetEntities(GameMatcher.Animation);
        foreach (var objEntity in entities.OrderBy(c => c.Position.Y))
        {
            target.Draw(objEntity.Animation.GetSprite());
        }

        target.Draw(_contexts.GameMap.GetForegroundTileMap());

#if DEBUG
        DrawCollisions(target);
#endif
    }

    private void DrawCollisions(RenderTarget target)
    {
        foreach (var item in _contexts.GameMap.MapData.CollidersLayer.Colliders)
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