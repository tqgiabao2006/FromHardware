using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lethal_Organization.UI;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;

    private Random _random;

    private SpriteBatch _spriteBatch;

    private Player _player;

    private Enemy _testEnemy;

    
    private EnemySpawner _enemySpawner;
    
    private Button _testButton;

    private SpriteFont _font;

    private GameManager _gameManager;

    private ObjectPooling _objectPool;

    private Level _level;

    //Player Sprite    
    private Texture2D _playerSpriteSheet;

    private Texture2D _bulletSprite;

    //Enemey
    private Texture2D _groundEnemySpriteSheet;

    private Texture2D _flyEnemySpriteSheet;

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
    private UIManager _uiManager;

    private Texture2D _UISprite;

    private Texture2D _startGameSprite;

    private Texture2D _exitSprite;

    private Texture2D _openScreenSPrite;

    private Texture2D _againSprite;

    private Texture2D _endTheme;

    private Texture2D _youDie;

    private Texture2D _credit;

    private Texture2D _creditMenu;

    private Texture2D _tutorial;

    private Texture2D _tutorialMenu;

    private Texture2D _exitButton;

    private int _screenWidth;

    private int _screenHeight;

    //SFX
    private Song _bossSong;

    private Song _endSong;

    private Song _mainSong;

    private Song _openSong;

    private SoundEffect _bossJump;

    private SoundEffect _buttonClick;
    private SoundEffect _iceCrack;
    private SoundEffect _spikeSound;
    private SoundEffect _playerJump;
    private SoundEffect _playerShoot;
    private SoundEffect _swoosh;
    private SoundEffect _dieSFX;
    private SoundEffect _getHitSFX;

    private AudioManager _audioManager;




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

        //SFX
        _bossSong = Content.Load<Song>(Constants.BossSong);

        _endSong = Content.Load<Song>(Constants.EndSong);

        _mainSong = Content.Load<Song>(Constants.MainSong);

        _openSong = Content.Load<Song>(Constants.OpenSong);

        _bossJump = Content.Load<SoundEffect>(Constants.BossJump);

        _buttonClick = Content.Load<SoundEffect>(Constants.ButtonClick);

        _iceCrack = Content.Load<SoundEffect>(Constants.IceCrack);

        _spikeSound = Content.Load<SoundEffect>(Constants.SpikeSound);

        _playerJump = Content.Load<SoundEffect>(Constants.PlayerJump);

        _playerShoot = Content.Load<SoundEffect>(Constants.PlayerShoot);

        _swoosh = Content.Load<SoundEffect>(Constants.Swoosh);

        _dieSFX = Content.Load<SoundEffect>(Constants.DieSFX);

        _getHitSFX = Content.Load<SoundEffect>(Constants.GetHit);

        //Boss
        _bossSpriteSheet = Content.Load<Texture2D>(Constants.BossSpriteSheet);

        _iceProjectile = Content.Load<Texture2D>(Constants.IceProjectileSprite);

        _iceSpike = Content.Load<Texture2D>(Constants.BossSpike);

        //Player
        _playerSpriteSheet = Content.Load<Texture2D>(Constants.PlayerSpriteSheet);

        _bulletSprite = Content.Load<Texture2D>(Constants.BulletSprite);

        //Enemy
        _groundEnemySpriteSheet = Content.Load<Texture2D>(Constants.GroundEnemySpriteSheet);

        _flyEnemySpriteSheet = Content.Load<Texture2D>(Constants.FlyEnemySpriteSheet);

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

        _tutorial = Content.Load<Texture2D>(Constants.Tutorial);

        _tutorialMenu = Content.Load<Texture2D>(Constants.TutorialMenu);

        _credit = Content.Load<Texture2D>(Constants.Credits);

        _creditMenu = Content.Load<Texture2D>( Constants.CreditsMenu);

        _againSprite = Content.Load<Texture2D>(Constants.Again);

        _endTheme = Content.Load<Texture2D>(Constants.EndTheme);

        _youDie = Content.Load<Texture2D>(Constants.YouDie);

        _exitButton = Content.Load<Texture2D>(Constants.ExitButton);

        //Window data
        _screenHeight = _graphics.GraphicsDevice.Viewport.Height;

        _screenWidth = _graphics.GraphicsDevice.Viewport.Width;

        _gameManager = new GameManager();

        _objectPool = ObjectPooling.Instance;

        _audioManager = new AudioManager(_gameManager,
            _bossSong, _endSong, _mainSong, _openSong, _bossJump, 
            _buttonClick, _iceCrack, _spikeSound, _playerJump, _playerShoot, _swoosh, _dieSFX, _getHitSFX);
        

        _level = new Level(_tileSpriteSheet, _skyBackground, _towerBackground, _collumBackground, _bossBackground,
            Constants.TextureMapTxt, Constants.LevelDesignCsv,
            3, 3, _screenWidth, _screenHeight, _gameManager);

        _player = new Player(_playerSpriteSheet, Constants.PlayerSpriteMap, _bulletSprite, _graphics, _level,_audioManager, _gameManager, _objectPool);

        _level.Player = _player;


        _boss = new Boss(_bossSpriteSheet, _iceProjectile, _iceSpike,  Constants.BossSpriteMap, _player, _level, _gameManager, _audioManager,_random, _objectPool);

        _player.GetBossAcess(_boss);

        _uiManager = new UIManager(_audioManager, _gameManager, _player, _boss,
         _font,
         _UISprite, _openScreenSPrite,_startGameSprite,_credit, _creditMenu, _tutorial, _tutorialMenu,_exitSprite,
        _youDie, _endTheme, _againSprite, _exitButton
         , _screenWidth, _screenHeight,
         Constants.MenuLayout, _gameManager.ChangeState, Exit);

        _enemySpawner = new EnemySpawner(_groundEnemySpriteSheet, _flyEnemySpriteSheet, _UISprite, _uiManager[UI.Type.EnemyHealthBar], Constants.EnemyPos,
            _level, _gameManager, _audioManager, _player);

        _player.EnemyList = _enemySpawner.EnemyList;

        _gameManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime);

        _enemySpawner.Update(gameTime);

        _gameManager.Update(gameTime);

        _boss.Update(gameTime);

        _uiManager.Update(gameTime);
        
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

        _uiManager.Draw(_spriteBatch);

        _enemySpawner.Draw(_spriteBatch);
        
        _boss.Draw(_spriteBatch);

   

        if (_gameManager.CurrentState == GameManager.GameState.Debug)
        {   
            //Debug-only
            _spriteBatch.DrawString(
            _font,
            $"State: {_gameManager.CurrentState} ",
            new Vector2(10, 10),
            Color.White);

            _spriteBatch.DrawString(
               _font,
               $"Player HP: {_player.CurHP} \n" +
               $"Floor:  {_player.LowestFloor}",
                new Vector2(10, 100),
               Color.Yellow
           );

            _spriteBatch.DrawString(
                _font,
                $"Boss state {_boss.BossState} " +
                $"\n Free {_boss.Free}" +
                $"\n Boss HP: {_boss.CurHP}",
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
                $"Boss Velcoity: {_boss.Velocity} \n" +
                $"On Command {_boss.OnCommand}",
                 new Vector2(10, 300),
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
                new Vector2(10, 400),
                Color.Aqua
                );


        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
