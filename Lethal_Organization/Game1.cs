using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Enemy testEnemy;

    //Test Player Sprite
    private Texture2D playerSprite;

    private Texture2D enemySprite;

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
            1, //Draw height scale
           1,  //Draw width scale
            _spriteBatch);

        playerSprite = Content.Load<Texture2D>("TempTexture");
        player = new Player(playerSprite);

        enemySprite = Content.Load<Texture2D>("TestEnemy");
        testEnemy = new Enemy(enemySprite);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        player.Update(gameTime);
        testEnemy.Update(gameTime);
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
        player.Draw(_spriteBatch);
        testEnemy.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
