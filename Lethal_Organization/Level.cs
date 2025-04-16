using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

/// <summary>
/// Collection of tile info
/// Save to fileIO the layout of level
/// 
/// </summary>
public class Level: IStateChange
{

    private enum LevelLayer
    {
        Main,
        BossRoom
    }
    private Texture2D _spriteSheet;

    private int _drawHeightScale; //Scale of the real image drawn in window for each tile. If tile 16x32, scale 2 => 32x64

    private int _drawWidthScale;
   
    private Player player;

    private Dictionary<string, Rectangle> _textureMap;
    
    private Tile[,] _levelDesign;

    Dictionary<LevelLayer, List<BackgroundLayer>> _backgroundLayers;
    
    private bool _visible;

    private bool _isDebug;

    private bool _paused;

    private int _screenWidth;

    private int _screenHeight;

    public Tile this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= _levelDesign.GetLength(0) || y < 0 || y >= _levelDesign.GetLength(1))
            {
                return null;
            }
            return _levelDesign[x, y];
        }
    }

    public int SizeX
    {
        get
        {
            if (_levelDesign != null)
            {
                return _levelDesign.GetLength(0);
            }

            return -1;
        }
    }

    public int SizeY
    {
        get
        {
            if (_levelDesign != null)
            {
                return _levelDesign.GetLength(1);
            }

            return -1;
        }
    }
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    public Level(Texture2D spriteSheet, Texture2D sky, Texture2D tower, Texture2D collum, Texture2D bossBackground,
        string textureMapFile, string levelDesignFile,
        int drawnHeightScale, int drawWidthScale, int screenWidth, int screenHeight,
        GameManager gameManager)
    {
        this._spriteSheet = spriteSheet;
    
        this._drawWidthScale = drawWidthScale;

        this._drawHeightScale = drawnHeightScale;

        this._screenHeight = screenHeight;

        this._screenWidth = screenWidth;
        
        _textureMap = new Dictionary<string, Rectangle>();

        _backgroundLayers = new Dictionary<LevelLayer, List<BackgroundLayer>>();

        _backgroundLayers.Add(LevelLayer.Main, new List<BackgroundLayer>());

        _backgroundLayers.Add(LevelLayer.BossRoom, new List<BackgroundLayer>());

        _backgroundLayers[LevelLayer.Main].Add(new BackgroundLayer()
        {
            Speed = 1,
            Texture = sky
        });


        _backgroundLayers[LevelLayer.Main].Add(new BackgroundLayer()
        {
            Speed = 0.5f,
            Texture = tower
        });


        _backgroundLayers[LevelLayer.Main].Add(new BackgroundLayer()
        {
            Speed = 0.8f,
            Texture = collum
        });

        _backgroundLayers[LevelLayer.BossRoom].Add(new BackgroundLayer()
        {
            Speed = 1f,
            Texture = bossBackground
        });

        gameManager.StateChangedAction += OnStateChange;
        
        InitializeTextureMap(textureMapFile);
        InitializeMapDesign(levelDesignFile);
    }


    public void OnStateChange(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:
                _paused = false;
                _visible = false;
                _isDebug = false;
                break;

            case GameManager.GameState.Game:
                _visible = true;
                _paused = false;
                _isDebug = false;

                break;

            case GameManager.GameState.GameOver:
                _visible = false;
                _paused = false;
                _isDebug = false;

                break;
            case GameManager.GameState.Pause:
                _paused = true;
                _visible = true;
                _isDebug = false;

                break;
            case GameManager.GameState.Debug:
                _paused = false;
                _visible = true;
                _isDebug = true;
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
    /// <param name="sb"></param>
    public void Draw(SpriteBatch sb, Vector2 cameraOffset)
    {
        if (_levelDesign == null || _levelDesign.GetLength(0) == 0) return;


        if (_visible)
        {
            if(_backgroundLayers.ContainsKey(LevelLayer.Main))
            {
                Rectangle displayPos = new Rectangle((int)cameraOffset.X - _screenWidth / 2 -200, (int)cameraOffset.Y - _screenHeight / 2, _screenWidth, _screenHeight);
                foreach (BackgroundLayer layer in _backgroundLayers[LevelLayer.Main])
                {
                    Rectangle layerPos = new Rectangle((int)(displayPos.X * layer.Speed), (int)(displayPos.Y), 1980, 1080);
                    sb.Draw(layer.Texture, new Vector2(layerPos.X, layerPos.Y), null, Color.White, 0, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 1);
                }
            }

            if (_backgroundLayers.ContainsKey(LevelLayer.BossRoom))
            {
                Rectangle displayPos = new Rectangle((int)cameraOffset.X - _screenWidth / 2, (int)cameraOffset.Y + 33 * 16 * _drawHeightScale, _screenWidth, _screenHeight);
                foreach (BackgroundLayer layer in _backgroundLayers[LevelLayer.BossRoom])
                {
                    Rectangle layerPos = new Rectangle((int)(displayPos.X * layer.Speed), (int)(displayPos.Y), 1980, 1080);
                    sb.Draw(layer.Texture, new Vector2(layerPos.X, layerPos.Y), null, Color.White, 0, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 1);
                }
            }



            for (int i = 0; i < _levelDesign.GetLength(0); i++)
            {
                for (int j = 0; j < _levelDesign.GetLength(1); j++)
                {
                    if (_levelDesign[i, j] != null)
                    {
                        _levelDesign[i, j].Draw(sb, _isDebug, player);
                    }
                }
            }
        }
    }


}