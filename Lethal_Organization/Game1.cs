using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    
    private SpriteBatch _spriteBatch;
    
    private Player _player;
    
    private Enemy _testEnemy;
    
    private Button _testButton;
    
    private Menu _menu;
   
    private SpriteFont _font;

    private GameManager _gameManager;

    private ObjectPooling _objectPool;
    
    //Test Player Sprite
    private Texture2D _playerSprite;
    
    private Texture2D _playerSpriteSheet;
    
    private Texture2D _enemySprite;
    
    private Texture2D _UISprite;

    private Texture2D _bulletSprite;

    //Level:
    private Texture2D _tileSpriteSheet;
    
    private Level _level;

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

        _playerSprite = Content.Load<Texture2D>(Constant.PlayerSpriteTexture);

        _playerSpriteSheet = Content.Load<Texture2D>(Constant.PlayerSpriteSheet);

        _enemySprite = Content.Load<Texture2D>(Constant.EnemySprite);

        _tileSpriteSheet = Content.Load<Texture2D>(Constant.TileSpriteSheet);

        _bulletSprite = Content.Load<Texture2D>(Constant.BulletSprite);

        _UISprite = Content.Load<Texture2D>(Constant.GUI);

        _font = Content.Load<SpriteFont>(Constant.Arial20);

        _screenHeight = _graphics.GraphicsDevice.Viewport.Height;

        _screenWidth = _graphics.GraphicsDevice.Viewport.Width;

        _gameManager = new GameManager(_font);

        _objectPool = ObjectPooling.Instance;

        _level = new Level(_tileSpriteSheet, Constant.TextureMapTxt, Constant.LevelDesignCsv, 3, 3, _gameManager);

        _player = new Player(_playerSprite, _bulletSprite,_graphics, _level, _gameManager, _objectPool);

        _level.Player = _player;
        _gameManager.Player = _player;

        _menu = new Menu(_UISprite, Constant.MenuLayout, new Vector2(_screenWidth / 2, _screenHeight / 2), _gameManager);

        _testEnemy = new Enemy(_enemySprite, _level[9, 2].DisplayPos, _player, _gameManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime);

        // Update button state
        //_testButton.Update(gameTime);

        _gameManager.Update(gameTime);
        
        _testEnemy.Update(gameTime);
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
        _level.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _menu.Draw(_spriteBatch);
        _gameManager.Draw(_spriteBatch);
        _testEnemy.Draw(_spriteBatch);
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
                _player._playerState.ToString(),
                new Vector2(_player.CameraPos.X, _player.CameraPos.Y),
                Color.Red);

            _spriteBatch.DrawString(
            _font,
            $"Velocity: {_player.Velocity.X}, {_player.Velocity.Y} \n \n Screen {_screenWidth}, {_screenHeight}",
                new Vector2(10, 50),
                Color.White);


            _spriteBatch.DrawString(
               _font,
               $"Offset: {_player.CameraOffset} \n World Pos: {_player.WorldPos.X}, {_player.WorldPos.Y} \n" +
               $"CameraPos: {_player.CameraPos.X}, {_player.CameraPos.Y}",
               new Vector2(10, 150),
               Color.Red);

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
                $"Bullet pool count: {_objectPool.BulletCount}",
                new Vector2(10, 300),
                Color.Aqua
                );

            foreach(Bullet bullet in _player.Bullets)
            {
                _spriteBatch.DrawString(
               _font,
               $"Speed {bullet.Speed}",
               bullet.WorldPos,
               Color.Red
               );
            }
           

        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
