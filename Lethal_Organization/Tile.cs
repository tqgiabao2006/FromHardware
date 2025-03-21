using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Tile: IDrawable
{
   private Rectangle _posRect;
   private Rectangle _sourceRect;
   private Texture2D _spriteSheet;

    public Rectangle PosRect
    {
        get { return _posRect; }
    }
       
    /// <summary>
    /// Constructs a LevelTile object
    /// </summary>
    /// <param name="posRect">Where will this tile be drawn in the game window?</param>
    /// <param name="rowIndex">Which row is the source tile in the sprite sheet?</param>
    /// <param name="colIndex">Which column is the source tile in the sprite sheet?</param>
    /// <param name = "width" > Pixel width of the tile
    /// <param name = "height" > Pixel height of the tile
    public Tile(Rectangle posRect, Rectangle sourceRect,Texture2D spriteSheet)
   {
       this._spriteSheet = spriteSheet;
       this._posRect = posRect;
       this._sourceRect = sourceRect;
   }
    

    /// <summary>
    /// Draw tile
    /// Expected to call sb.Begin()/End() elsewhere
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch, bool isDebug)
    {
       
        spriteBatch.Draw(
            _spriteSheet,
            _posRect,
            _sourceRect,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
            );

        if (isDebug)
        {
            CustomDebug.DrawRectOutline(spriteBatch, this._posRect, 3, Color.Blue);
        }
    }
   

}