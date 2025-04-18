using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Tile
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
    public void Draw(SpriteBatch sb, bool isDebug, Player player)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)player.CameraOffset.X, _worldPos.Y + (int)player.CameraOffset.Y, _worldPos.Width, _worldPos.Height);

        sb.Draw(
            _spriteSheet,
            _displayPos,
            _sourceImg,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            1
            );
        
        if (isDebug)
        {
            if (DisplayPos.Intersects(player.CameraPos))
            {
                CustomDebug.DrawWireRectangle(sb, _displayPos, 1f, Color.Red);
                CustomDebug.DrawWireRectangle(sb, _worldPos, 2f, Color.Red);
            }
            else
            {
                CustomDebug.DrawWireRectangle(sb, _displayPos, 1f, Color.Green);
                CustomDebug.DrawWireRectangle(sb, _worldPos, 2f, Color.BlueViolet);
            }
        }
    }
}