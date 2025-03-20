using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player player;
    private Button _testButton;

    //Test Player Sprite
    private Texture2D playerSprite;

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

        // Load and initialize the test button
        _testButton = new Button(Content, new Vector2(100, 100));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update button state
        _testButton.Update(gameTime);

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

        // Draw the test button
        _testButton.Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
