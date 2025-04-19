using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;

    private Random _random;

    private SpriteBatch _spriteBatch;
    
    private Player _player;
    
    private EnemySpawner _enemySpawner;
    
    private Button _testButton;
    
    private Menu _menu;
   
    private SpriteFont _font;

    private GameManager _gameManager;

    private ObjectPooling _objectPool;

    private Level _level;

    //Player Sprite    
    private Texture2D _playerSpriteSheet;
    
    private Texture2D _enemySprite;
    
    private Texture2D _bulletSprite;

    //Level:

    private Texture2D _tileSpriteSheet;

    private Texture2D _skyBackground;

    private Texture2D _towerBackground;

    private Texture2D _collumBackground;

    private Texture2D _bossBackground;

    //Boss
    private Texture2D _bossSpriteSheet;

    private Texture2D _iceProjectile;

    private Texture2D _iceSpike;

    private Boss _boss;

    //UI
    private Texture2D _UISprite;

    private Texture2D _startGameSprite;

    private Texture2D _loadGameSprite;

    private Texture2D _exitSprite;

    private Texture2D _optionSprite;

    private Texture2D _openScreenSPrite;

    private int _screenWidth;
    
    private int _screenHeight;
    

  

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {

        base.Initialize();

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _random = new Random();

        //Boss
        _bossSpriteSheet = Content.Load<Texture2D>(Constants.BossSpriteSheet);

        _iceProjectile = Content.Load<Texture2D>(Constants.IceProjectileSprite);

        _iceSpike = Content.Load<Texture2D>(Constants.BossSpike);

        //Player
        _playerSpriteSheet = Content.Load<Texture2D>(Constants.PlayerSpriteSheet);

        _bulletSprite = Content.Load<Texture2D>(Constants.BulletSprite);

        //Enemy
        _enemySprite = Content.Load<Texture2D>(Constants.EnemySprite);

        //Level_Map
        _tileSpriteSheet = Content.Load<Texture2D>(Constants.TileSpriteSheet);

        _skyBackground = Content.Load<Texture2D>(Constants.Sky);

        _collumBackground = Content.Load<Texture2D>(Constants.Collum);

        _towerBackground = Content.Load<Texture2D>(Constants.Tower);

        _bossBackground = Content.Load<Texture2D>(Constants.BossBackground);

        //UI
        _UISprite = Content.Load<Texture2D>(Constants.GUI);

        _font = Content.Load<SpriteFont>(Constants.Arial20);

        _openScreenSPrite = Content.Load<Texture2D>(Constants.OpenTheme);

        _startGameSprite = Content.Load<Texture2D>(Constants.StartGame);

        _exitSprite = Content.Load<Texture2D>(Constants.Exit);

        _optionSprite = Content.Load<Texture2D>(Constants.Options);

        _loadGameSprite = Content.Load<Texture2D>(Constants.LoadGame);

        //Window data
        _screenHeight = _graphics.GraphicsDevice.Viewport.Height;

        _screenWidth = _graphics.GraphicsDevice.Viewport.Width;

        _gameManager = new GameManager(_font);

        _objectPool = ObjectPooling.Instance;

        _level = new Level(_tileSpriteSheet,_skyBackground, _towerBackground, _collumBackground, _bossBackground,
            Constants.TextureMapTxt, Constants.LevelDesignCsv, 
            3, 3, _screenWidth, _screenHeight, _gameManager);

        _player = new Player(_playerSpriteSheet, Constants.PlayerSpriteMap,  _bulletSprite,_graphics, _level, _gameManager, _objectPool);

        _level.Player = _player;

        _gameManager.Player = _player;

        _menu = new Menu(_UISprite, _openScreenSPrite, _loadGameSprite, _startGameSprite, _exitSprite, _optionSprite,
            _screenWidth, _screenHeight,
            Constants.MenuLayout, new Vector2(850, 350), _gameManager, _gameManager.ChangeState);

        //_testEnemy = new Enemy(_enemySprite, _level[9, 2].DisplayPos, _player, _gameManager);
        _enemySpawner = new EnemySpawner(_enemySprite, _enemySprite, "EnemyPositions", _level);

        _boss = new Boss(_bossSpriteSheet, _iceProjectile, Constants.BossSpriteMap, _player, _level, _gameManager, _random, _objectPool);

        _gameManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime);

        _enemySpawner.Update(gameTime);

        // Update button state
        //_testButton.Update(gameTime);

        _gameManager.Update(gameTime);

        _boss.Update(gameTime);

        _menu.Update(gameTime);
        
        //_testEnemy.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            null,
            SamplerState.PointClamp, // Prevents texture blurring because we do pixel art
            null,
            null
        );
        
        _level.Draw(_spriteBatch, _player.CameraOffset);
       
        _player.Draw(_spriteBatch);

        _enemySpawner.Draw(_spriteBatch);
        
        _menu.Draw(_spriteBatch);
        
        _gameManager.Draw(_spriteBatch);

        _boss.Draw(_spriteBatch);

        //_testEnemy.Draw(_spriteBatch, _player);
        // Draw the test button
        //_testButton.Draw(_spriteBatch, true);


        if(_gameManager.CurrentState == GameManager.GameState.Debug)
        {
            _spriteBatch.DrawString(
            _font,
            $"On ground: {_player.OnGround} ",
            new Vector2(10, 10),
            Color.White);

            _spriteBatch.DrawString(
                _font,
                $"Boss state {_boss.BossState}",
                new Vector2(10, 200),
                Color.Yellow
                );
            //_spriteBatch.DrawString(
            //    _font,
            //    _player._playerState.ToString(),
            //    new Vector2(_player.CameraPos.X, _player.CameraPos.Y),
            //    Color.Red);

            //_spriteBatch.DrawString(
            //_font,
            //$"Velocity: {_player.Velocity.X}, {_player.Velocity.Y} \n \n Screen {_screenWidth}, {_screenHeight}",
            //    new Vector2(10, 50),
            //    Color.White);


            //_spriteBatch.DrawString(
            //   _font,
            //   $"Offset: {_player.CameraOffset} \n World Pos: {_player.WorldPos.X}, {_player.WorldPos.Y} \n" +
            //   $"CameraPos: {_player.CameraPos.X}, {_player.CameraPos.Y}",
            //   new Vector2(10, 150),
            //   Color.Red);
            

            _spriteBatch.DrawString(
                _font,
                $"Boss Velcoity: {_boss.Velocity} \n \n" +
                $"On Command {_boss.OnCommand}",
                 new Vector2(10, 200),
                Color.Yellow
            );

            _spriteBatch.DrawString(
                _font,
                ".",
                _player.GroundCheckPoint,
                Color.Aqua);

            _spriteBatch.DrawString(
               _font,
               ".",
               _player.RightRayPoint,
               Color.Aqua);

            _spriteBatch.DrawString(
               _font,
               ".",
               _player.LeftRayPoint,
               Color.Aqua);

            _spriteBatch.DrawString(
                _font,
                $"Bullet boss pool count: {_objectPool.GetBulletCount(ObjectPooling.ProjectileType.BossBullet)}",
                new Vector2(10, 300),
                Color.Aqua
                );


        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
