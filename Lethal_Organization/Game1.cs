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
    
    //Test Player Sprite
    private Texture2D _playerSprite;
    private Texture2D _playerSpriteSheet;

    private Texture2D _enemySprite;

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
        _playerSprite = Content.Load<Texture2D>("TempTexture");
        _playerSpriteSheet = Content.Load<Texture2D>("PlayerSpriteSheet");
        _player = new Player(_playerSprite, _graphics, _level);
        //Test-only
        _tileSpriteSheet = Content.Load<Texture2D>("TileSpriteSheet");
        _level = new Level(
            _tileSpriteSheet, //Sprite sheet
            "../../../Content/textureMap.txt",  //Texture map file path
            "../../../Content/LevelDesign.csv", //Level design file path
            4, //Draw height scale
           4,  //Draw width scale
            _spriteBatch);
        _player = new Player(_playerSprite, _graphics, _level);
        _font = Content.Load<SpriteFont>("Arial20");


        _menu = new Menu(Content.Load<Texture2D>("GUI"), "../../../Content/menuUI.txt", new Vector2(_screenWidth/2, _screenHeight/2));


        _enemySprite = Content.Load<Texture2D>("TestEnemy");
        _testEnemy = new Enemy(_enemySprite, _level.LevelDesign[9,2].DisplayPos, _player);


        _screenHeight = _graphics.GraphicsDevice.Viewport.Height;
        _screenWidth = _graphics.GraphicsDevice.Viewport.Width;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime, _level.LevelDesign);

        // Update button state
        //_testButton.Update(gameTime);
        
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
        _level.Draw(_spriteBatch, true);
        _player.Draw(_spriteBatch, true);
        _menu.Draw(_spriteBatch, true);
        // Draw the test button
        //_testButton.Draw(_spriteBatch, true);


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
           $"CameraPos: {_player.CameraPos.X}, {_player.CameraPos.Y}" ,
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

        _testEnemy.Draw(_spriteBatch, true);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
