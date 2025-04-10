using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Tile : IDrawable
{
    private Rectangle _worldPos;

    private Rectangle _displayPos;
    
    private Rectangle _sourceImg;
    
    private Texture2D _spriteSheet;

    public Rectangle WorldPos
    {
        get { return _worldPos; }
    }
    public Rectangle DisplayPos
    {
        get { return _displayPos; }
    }

    /// <summary>
    /// Constructs a LevelTile object
    /// </summary>
    /// <param name="posRect">Where will this tile be drawn in the game window?</param>
    /// <param name="rowIndex">Which row is the source tile in the sprite sheet?</param>
    /// <param name="colIndex">Which column is the source tile in the sprite sheet?</param>
    /// <param name = "width" > Pixel width of the tile
    /// <param name = "height" > Pixel height of the tile
    public Tile(Rectangle worldPos, Rectangle sourceRect, Texture2D spriteSheet)
    {
        this._spriteSheet = spriteSheet;
        this._worldPos = worldPos;
        this._sourceImg = sourceRect;
        _displayPos = _worldPos;
    }


    /// <summary>
    /// Draw tile
    /// Expected to call sb.Begin()/End() elsewhere
    /// </summary>
    /// <param name="sb"></param>
    public void Draw(SpriteBatch sb, bool isDebug, Vector2 offset)
    {
        //_posRect.X += (int)offset.X;
        //_posRect.Y += (int)offset.Y;

        _displayPos = new Rectangle(_worldPos.X + (int)offset.X, _worldPos.Y + (int)offset.Y, _worldPos.Width, _worldPos.Height);

        sb.Draw(
            _spriteSheet,
            _displayPos,
            _sourceImg,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
            );

        if (isDebug)
        {
            CustomDebug.DrawWireRectangle(sb, _displayPos, 1f, Color.Green);

            CustomDebug.DrawWireRectangle(sb, _worldPos, 2f, Color.BlueViolet);
        }
    }

    /// <summary>
    /// Do not use
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="isDebug"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void Draw(SpriteBatch sb, bool isDebug)
    {
        throw new System.NotImplementedException();
    }
}