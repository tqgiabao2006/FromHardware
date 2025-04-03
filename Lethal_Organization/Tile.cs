using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Tile : IDrawable
{
    private Rectangle _spawnRect;
    private Rectangle _screenRect;
    private Rectangle _sourceRect;
    private Texture2D _spriteSheet;

    public Rectangle ScreenRec
    {
        get { return _screenRect; }
    }

    /// <summary>
    /// Constructs a LevelTile object
    /// </summary>
    /// <param name="posRect">Where will this tile be drawn in the game window?</param>
    /// <param name="rowIndex">Which row is the source tile in the sprite sheet?</param>
    /// <param name="colIndex">Which column is the source tile in the sprite sheet?</param>
    /// <param name = "width" > Pixel width of the tile
    /// <param name = "height" > Pixel height of the tile
    public Tile(Rectangle spawnRect, Rectangle sourceRect, Texture2D spriteSheet)
    {
        this._spriteSheet = spriteSheet;
        this._spawnRect = spawnRect;
        this._sourceRect = sourceRect;
        _screenRect = _spawnRect;
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

        _screenRect = new Rectangle(_spawnRect.X + (int)offset.X, _spawnRect.Y + (int)offset.Y, _spawnRect.Width, _spawnRect.Height);


        sb.Draw(
            _spriteSheet,
            _screenRect,
            _sourceRect,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
            );

        if (isDebug)
        {
            CustomDebug.DrawWireRectangle(sb, _screenRect, 0.5f, Color.Aqua);
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