using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Tile
{
   private Rectangle _posRect;
   private Rectangle _sourceRect;
   private Texture2D _spriteSheet;
       
    /// <summary>
    /// Constructs a LevelTile object
    /// </summary>
    /// <param name="posRect">Where will this tile be drawn in the game window?</param>
    /// <param name="rowIndex">Which row is the source tile in the sprite sheet?</param>
    /// <param name="colIndex">Which column is the source tile in the sprite sheet?</param>
    /// <param name = "width" > Pixel width of the tile
    /// <param name = "height" > Pixel height of the tile
    public Tile(Rectangle posRect, Texture2D spriteSheet, int rowIndex, int colIndex, int width, int height)
   {
       this._spriteSheet = spriteSheet;
       this._posRect = posRect;
       this._sourceRect = CalculateSourceRect(spriteSheet, rowIndex, colIndex, width, height);
   }

    
    /// <summary>
    /// STANDARD PIXEL OF COLUMNS AND ROWS: 16X16
    /// CalculateSourceRect base on row index, col index, pixel rate
    /// No space between each tile
    /// </summary>
    /// <param name="spriteSheet"></param>
    /// <param name="rowIndex"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    private Rectangle CalculateSourceRect(Texture2D spriteSheet, int rowIndex, int colIndex, int width, int height)
    {
        return new Rectangle
        (rowIndex * 16   //Standard pixel in sprite sheet is 16x16
        , colIndex * 16
        , width, height);
    }



    /// <summary>
    /// Draw tile
    /// Expected to call sb.Begin()/End() elsewhere
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
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
    }
   

}