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
public class Level
{
    private SpriteBatch _spriteBatch; 
        
    private Texture2D _spriteSheet;
    private Tile[,] _map;
    private int _drawHeightScale; //Scale of the real image drawn in window for each tile. If tile 16x32, scale 2 => 32x64
    private int _drawWidthScale;

    private Dictionary<string, Rectangle> _textureMap;

    public Level(Texture2D spriteSheet, string textureMapFile, int drawnHeightScale, int drawWidthScale, SpriteBatch spriteBatch)
    {
        this._spriteSheet = spriteSheet;
        this._spriteBatch = spriteBatch;
        
        _textureMap = new Dictionary<string, Rectangle>();

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
                        new Rectangle(
                            int.Parse(data[2]) * 16, //X-pivot = ColIndex * 16 (16 is standard pixel scale, no space between tile)
                            int.Parse(data[1]) * 16, //Y-pivot = RowIndex * 16 
                            int.Parse(data[2]) * 16 + currentWidth, //Width = X-pivot + current width
                            int.Parse(data[1]) * 16 + currentWidth) //Height = Y-Pivot + current height
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
}