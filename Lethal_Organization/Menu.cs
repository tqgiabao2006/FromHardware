using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    internal class Menu : IDrawable
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _spriteSheet;
        private Dictionary<string, Rectangle> _textureMap;
        private List<(Rectangle buttonRect, Rectangle textRect)> _menuItems;
        private Vector2 _position;
        private int _scale;

        public Menu(Texture2D spriteSheet, string textureMapFile, SpriteBatch spriteBatch, Vector2 position, int scale = 2)
        {
            _spriteSheet = spriteSheet;
            _spriteBatch = spriteBatch;
            _position = position;
            _scale = scale;
            _textureMap = new Dictionary<string, Rectangle>();
            _menuItems = new List<(Rectangle, Rectangle)>();

            InitializeTextureMap(textureMapFile);
            InitializeMenuLayout();
        }

        private void InitializeTextureMap(string textureMapFile)
        {
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(textureMapFile);
                string line;
                int currentWidth = 0;
                int currentHeight = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("/") || line.StartsWith("=") || line == "") continue;

                    var data = line.Split(',');

                    if (data.Length == 2) // Size line
                    {
                        currentWidth = int.Parse(data[0]);
                        currentHeight = int.Parse(data[1]);
                    }
                    else if (data.Length == 2) continue; // Skip malformed
                    else if (data.Length == 1) continue; // Skip malformed
                    else if (data.Length == 3) // Element line
                    {
                        _textureMap[data[0]] = new Rectangle(
                            int.Parse(data[2]) * 16,
                            int.Parse(data[1]) * 16,
                            currentWidth,
                            currentHeight
                        );
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load texture map: " + e.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        private void InitializeMenuLayout()
        {
            // Buttons and matching text names (must match keys in textureMap)
            var buttonTextPairs = new List<(string button, string text)>
            {
                ("LargeButton", "NewText"),
                ("LargeButton", "LoadText"),
                ("LargeButton", "OptionsText"),
                ("LargeButton", "ExitText")
            };

            int yOffset = 0;
            foreach (var pair in buttonTextPairs)
            {
                if (_textureMap.ContainsKey(pair.button) && _textureMap.ContainsKey(pair.text))
                {
                    var buttonSource = _textureMap[pair.button];
                    var textSource = _textureMap[pair.text];

                    var buttonDest = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y + yOffset,
                        buttonSource.Width * _scale,
                        buttonSource.Height * _scale
                    );

                    var textDest = new Rectangle(
                        buttonDest.X + (buttonDest.Width - textSource.Width * _scale) / 2,
                        buttonDest.Y + (buttonDest.Height - textSource.Height * _scale) / 2,
                        textSource.Width * _scale,
                        textSource.Height * _scale
                    );

                    _menuItems.Add((buttonDest, textDest));
                    yOffset += buttonDest.Height + 10; // Padding between buttons
                }
            }
        }

        public void LoadContent(Texture2D spriteSheet, string textureMapFile)
        {
            _spriteSheet = spriteSheet;
            _textureMap = LoadTextureMap(textureMapFile);

            InitializeMenuLayout();
        }

        private Dictionary<string, Rectangle> LoadTextureMap(string textureMapFile)
        {
            var map = new Dictionary<string, Rectangle>();

            // Read each line in the texture map file
            foreach (var line in File.ReadAllLines(textureMapFile))
            {
                var parts = line.Split(',');

                if (parts.Length == 3) // Only care about lines with valid data
                {
                    string key = parts[0].Trim();
                    int x = int.Parse(parts[1].Trim());
                    int y = int.Parse(parts[2].Trim());

                    // Assuming the width and height of each tile are known (e.g., 16x32)
                    map[key] = new Rectangle(x * 16, y * 16, 16, 32); // Adjust size based on your tile size
                }
            }

            return map;
        }

        public void Draw(SpriteBatch sb, bool isDebug)
        {
            foreach (var (buttonRect, textRect) in _menuItems)
            {
                // draw button
                sb.Draw(_spriteSheet, buttonRect, _textureMap["LargeButton"], Color.White);

                // determine which text was intended for this button by its Y position
                string textKey = GetTextKeyByY(textRect.Y);
                if (textKey != null)
                {
                    sb.Draw(_spriteSheet, textRect, _textureMap[textKey], Color.White);
                }
            }
        }

        private string GetTextKeyByY(int y)
        {
            foreach (var key in _textureMap.Keys)
            {
                if (key.Contains("Text") && _textureMap[key].Height == 32)
                {
                    if (y.ToString().Contains("0")) // hacky placeholder logic
                        return key;
                }
            }
            return null;
        }
    }
}
