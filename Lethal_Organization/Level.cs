using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

/// <summary>
/// Collection of tile info
/// Save to fileIO the layout of level
/// 
/// </summary>
public class Level
{
    private SpriteBatch _spriteBatch; 
        
    private Texture2D _spriteSheet;
    private Tile[,] _map;
    private int _drawnSize; //Size of the real image drawn in window for each tile

    private Dictionary<string, Rectangle> _textureMap;

    public Level(Texture2D spriteSheet, string textureMapFile, SpriteBatch spriteBatch)
    {
        this._spriteSheet = spriteSheet;
        this._spriteBatch = spriteBatch;
        
        //In sprite sheet, each tile is 15x16 but drawn as 32x32 in screen
        _drawnSize = 32;
        
        _textureMap = new Dictionary<string, Rectangle>(); 
        
    }

}