using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

/// <summary>
/// Collection of tile info
/// Save to fileIO the layout of level
/// 
/// </summary>
public class Level: IDrawable, IStateChange
{
        
    private Texture2D _spriteSheet;
   
    private int _drawHeightScale; //Scale of the real image drawn in window for each tile. If tile 16x32, scale 2 => 32x64

    private int _drawWidthScale;
   
    private Vector2 _offset;

    private Dictionary<string, Rectangle> _textureMap;
    
    private Tile[,] _levelDesign;
    
    protected bool visible;

    protected bool isDebug;

    protected bool paused;

    public Tile[,] LevelDesign
    {
        get { return _levelDesign; }
    }
    public Vector2 Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }


    public Level(Texture2D spriteSheet, string textureMapFile, string levelDesignFile, int drawnHeightScale, int drawWidthScale, GameManager gameManager)
    {
        this._spriteSheet = spriteSheet;
        this._drawWidthScale = drawWidthScale;
        this._drawHeightScale = drawnHeightScale;
        
        _textureMap = new Dictionary<string, Rectangle>();

        gameManager.StateChangedAction += OnStateChange;
        
        InitializeTextureMap(textureMapFile);
        InitializeMapDesign(levelDesignFile);
    }


    public void OnStateChange(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:
                paused = false;
                visible = false;
                isDebug = false;
                break;

            case GameManager.GameState.Game:
                visible = true;
                paused = false;
                isDebug = false;

                break;

            case GameManager.GameState.GameOver:
                visible = false;
                paused = false;
                isDebug = false;

                break;
            case GameManager.GameState.Pause:
                paused = true;
                visible = true;
                isDebug = false;

                break;
            case GameManager.GameState.Debug:
                paused = false;
                visible = true;
                isDebug = true;
                break;
        }
    }

    /// <summary>
    /// Read and save each sprite to dictionary as a rectangle 
    /// </summary>
    /// <param name="textureMapFile"></param>
    private void InitializeTextureMap(string textureMapFile)
    {
        StreamReader reader = null;
        try
        {
            reader = new StreamReader(textureMapFile);
            string line = "";

            int currentWidth = 0;
            int currentHeight = 0;

            while ((line = reader.ReadLine()) != null)
            {
                //Skip the line start by '/' or '=' because it is description, not data
                if (line[0] == '/' || line[0] == '=')
                {
                    continue;
                }

                string[] data = line.Split(',');

                //If it is size data
                if (data.Length == 2)
                {
                    currentWidth = int.Parse(data[0]);
                    currentHeight = int.Parse(data[1]);
                }
                else if (data.Length == 3)
                {
                    _textureMap.Add(
                        data[0], //Tile Name
                        new Rectangle( //Source Rect
                            int.Parse(data[2]) * 16, //X-pivot = ColIndex * 16 (16 is standard pixel scale, no space between tile)
                            int.Parse(data[1]) * 16, //Y-pivot = RowIndex * 16 
                            currentWidth, 
                            currentHeight)
                    );
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("ERROR: Can not find the text file!");
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
    }

    /// <summary>
    /// Read csv file to initialize the tile() object, save to array[,] map, ready to drawn in the window
    /// </summary>
    private void InitializeMapDesign(string levelDesignFile)
    {
        StreamReader reader = null;
        
        try
        {
            int currentRow = 0;
            reader = new StreamReader(levelDesignFile);

            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line[0] == '/' || line[0] == '=')
                {
                    continue;
                }
                
                string[] data = line.Split(',');
                if (data.Length == 2 && _levelDesign == null)
                {
                    _levelDesign = new Tile[int.Parse(data[0]), int.Parse(data[1])]; //Initialize with 2D array row x col
                }
                else if(data.Length > 2)
                {
                    for (int col = 0; col < data.Length; col++)
                    {
                        if (data[col] != "")
                        {
                            Rectangle sourceRec = _textureMap[data[col]];
                            _levelDesign[currentRow, col] = new Tile
                            (
                                //PosRec
                                new Rectangle(col * 16 * _drawHeightScale, 
                                                     currentRow * 16 * _drawHeightScale, 
                                                     sourceRec.Width *_drawWidthScale,
                                                     sourceRec.Height * _drawHeightScale),
                                sourceRec,
                                _spriteSheet
                            );
                        }
                    }
                    
                    currentRow++;
                }
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
      finally
      {
          if (reader != null)
          {
              reader.Close();
          }
      }           
    }

    /// <summary>
    /// Draw tile
    /// Expected to call sb.Begin()/End() elsewhere
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_levelDesign == null || _levelDesign.GetLength(0) == 0) return;


        if (visible)
        {
            for (int i = 0; i < _levelDesign.GetLength(0); i++)
            {
                for (int j = 0; j < _levelDesign.GetLength(1); j++)
                {
                    if (_levelDesign[i, j] != null)
                    {
                        _levelDesign[i, j].Draw(spriteBatch, isDebug, _offset);
                    }
                }
            }


        }
    }


}