﻿using Microsoft.Xna.Framework;
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

    private SpriteFont _font;

    //Test Player Sprite
    private Texture2D _playerSprite;

    private Texture2D _enemySprite;

    //Level:
    private Texture2D _tileSpriteSheet;
    private Level _level;

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
        //Test-only
        _tileSpriteSheet = Content.Load<Texture2D>("TileSpriteSheet");
        _level = new Level(
            _tileSpriteSheet, //Sprite sheet
            "../../../Content/textureMap.txt",  //Texture map file path
            "../../../Content/LevelDesign.csv", //Level design file path
            3, //Draw height scale
           3,  //Draw width scale
            _spriteBatch);

        _font = Content.Load<SpriteFont>("Arial20");

        _playerSprite = Content.Load<Texture2D>("TempTexture");
        _player = new Player(_playerSprite, _level.LevelDesign);

        // Load and initialize the test button
        _testButton = new Button(Content, new Vector2(100, 100));

        _enemySprite = Content.Load<Texture2D>("TestEnemy");
        _testEnemy = new Enemy(_enemySprite);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        _player.Update(gameTime);

        // Update button state
        _testButton.Update(gameTime);

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

        // Draw the test button
        _testButton.Draw(_spriteBatch, true);

        if (_player._onGround)
        {
            _spriteBatch.DrawString(
                _font,
                "On ground: True",
                new Vector2(10, 10),
                Color.White);
        }
        else
        {
            _spriteBatch.DrawString(
                _font,
                "On ground: False",
                new Vector2(10, 10),
                Color.White);
        }
        
        _spriteBatch.DrawString(
            _font,
            _player._playerState.ToString(),
            new Vector2(_player.Position.X, _player.Position.Y),
            Color.Red);

        _spriteBatch.DrawString(
                     _font,
                    $"Velocity: {_player.Velocity.X}, {_player.Velocity.Y}",
                     new Vector2(10, 50),
                     Color.White);



        _testEnemy.Draw(_spriteBatch, true);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
