using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Enemy _testEnemy;
    private Button _testButton;

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

    // Test Menu
    private Menu _menu;


   

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

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

        // Load sprite sheet (e.g., menuUI)
        Texture2D spriteSheet = Content.Load<Texture2D>("GUI");

        // Define the necessary parameters for the Menu constructor
        string textureMapFile = "../../../Content/menuUI.txt"; // Path to your texture map file
        Vector2 offset = Vector2.Zero; // You can adjust the offset as needed
        int drawScale = 3; // Example scale factor for drawing

        // Initialize Menu object and load content
        _menu = new Menu(spriteSheet, textureMapFile, _spriteBatch, offset, drawScale);
        _playerSprite = Content.Load<Texture2D>("TempTexture");
        _playerSpriteSheet = Content.Load<Texture2D>("PlayerSpriteSheet");
        _player = new Player(_playerSprite, _graphics, _level);
        //Test-only
        _tileSpriteSheet = Content.Load<Texture2D>("TileSpriteSheet");
        _level = new Level(
            _tileSpriteSheet, //Sprite sheet
            "../../../Content/textureMap.txt",  //Texture map file path
            "../../../Content/LevelDesign.csv", //Level design file path
            3, //Draw height scale
           3,  //Draw width scale
            _spriteBatch);
        _player = new Player(_playerSprite, _graphics, _level);
        _font = Content.Load<SpriteFont>("Arial20");

        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime, _level.LevelDesign);

       


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        // Draw the menu
        _menu.Draw(_spriteBatch, true);

        if (_player._onGround)
        {
            _spriteBatch.DrawString(
                _font,
                "On ground: True \n" +             
                $"Ray Cast Hit: {_player.RayCastHit}",
                new Vector2(10, 10),
                Color.White);
        }
        else
        {
            _spriteBatch.DrawString(
                _font,
                "On ground: False \n " +
                $"Ray Cast Hit: {_player.RayCastHit}",
                new Vector2(10, 10),
                Color.White);
        }

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
            "*",
            _player.RayPoint,
            Color.Aqua);



        _spriteBatch.End();
        base.Draw(gameTime);
    }
    private Dictionary<string, Rectangle> LoadTextureMap(string filePath)
    {
        var map = new Dictionary<string, Rectangle>();
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(' ');
            string key = parts[0];
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            int width = int.Parse(parts[3]);
            int height = int.Parse(parts[4]);

            map[key] = new Rectangle(x, y, width, height);
        }
        return map;
    }
}
