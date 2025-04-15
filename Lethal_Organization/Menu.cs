
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    internal class Menu : IDrawable
    {
        public enum Type
        {
            AmmoBar,
            HealthBar,
            BarPlaceholder,
            MedButtonPlaceholder,
            MedButton,
            TextBox,
            SmallButton,
            SmallButtonPress,
            SmallButtonHover,
            HomeIcon,
            PauseIcon,
            SaveText,
            LoadText,
            NewText,
            BackText, 
            ExitText,
            OptionsText,
            CreditsText,
            SoundText,
            PlayerIcon,
            LargeButton,
            LargeButtonHover,
            LargeButtonPress,
            PauseMenu
        }

        private Texture2D _spriteSheet;
        
        private Dictionary<Type, Rectangle> _textureMap;
        
        private List<(Rectangle textPos, Rectangle buttonPos)> _menuItems;
       
        private Vector2 _position;
        
        private int _scale;
        
        protected bool visible;

        protected bool isDebug;

        protected bool paused;


        public Menu(Texture2D spriteSheet, string textureMapFile, Vector2 position, GameManager gameManager,int scale = 5)
        {
            _spriteSheet = spriteSheet;
            _position = position;
            _scale = scale;
            _textureMap = new Dictionary<Type, Rectangle>();
            _menuItems = new List<(Rectangle, Rectangle)>();

            gameManager.StateChangedAction += OnStateChange;

            InitializeTextureMap(textureMapFile);
            InitializeMenuLayout();
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

                        _textureMap.Add(
                           (Type)Enum.Parse(typeof(Type), data[0]),
                           new Rectangle(
                            int.Parse(data[2]) * 16,
                            int.Parse(data[1]) * 16,
                            currentWidth,
                            currentHeight
                        ));
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
            var buttonTextPairs = new List<(Type button, Type text)>
            {
                (Type.LargeButton, Type.NewText),
                (Type.LargeButton, Type.LoadText),
                (Type.LargeButton, Type.OptionsText),
                (Type.LargeButton, Type.ExitText)
            };
            var buttonOne = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            var textOne = new Rectangle(
                        buttonOne.X + (buttonOne.Width - _textureMap[Type.NewText].Width * _scale) / 2,
                        buttonOne.Y + (buttonOne.Height - _textureMap[Type.NewText].Height * _scale) / 2 + 10,
                        _textureMap[Type.NewText].Width * 4,
                         _textureMap[Type.NewText].Height * 4
                    );
            _menuItems.Add((buttonOne, textOne));

            var buttonTwo = new Rectangle(
                        (int)_position.X + 30,
                        (int)_position.Y + 30,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            var textTwo = new Rectangle(
                       buttonTwo.X + (buttonTwo.Width - _textureMap[Type.NewText].Width * _scale) / 2,
                       buttonTwo.Y + (buttonTwo.Height - _textureMap[Type.NewText].Height * _scale) / 2 + 10,
                       _textureMap[Type.NewText].Width * 4,
                        _textureMap[Type.NewText].Height * 4
                        );
            _menuItems.Add((buttonTwo, textTwo));

            var buttonThree = new Rectangle(
                        (int)_position.X + 60,
                        (int)_position.Y + 60,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            var textThree = new Rectangle(
                       buttonThree.X + (buttonThree.Width - _textureMap[Type.NewText].Width * _scale) / 2,
                       buttonThree.Y + (buttonThree.Height - _textureMap[Type.NewText].Height * _scale) / 2 + 10,
                       _textureMap[Type.NewText].Width * 4,
                        _textureMap[Type.NewText].Height * 4
                        );

            var buttonFour = new Rectangle(
                        (int)_position.X + 90,
                        (int)_position.Y + 90,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            foreach (var pair in buttonTextPairs)
            {
                if (_textureMap.ContainsKey(pair.button) && _textureMap.ContainsKey(pair.text))
                {
                    var buttonSource = _textureMap[pair.button];
                    var textSource = _textureMap[pair.text];

                    var buttonDest = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y,
                        buttonSource.Width * 4,
                        buttonSource.Height * 4
                    );

                    var textDest = new Rectangle(
                        buttonDest.X + (buttonDest.Width - textSource.Width * _scale) / 2,
                        buttonDest.Y + (buttonDest.Height - textSource.Height* _scale) / 2 +10,
                        textSource.Width * 4,
                        textSource.Height * 4
                    );

                    _menuItems.Add((buttonDest, textDest));
                }
            }
        }

        public void LoadContent(Texture2D spriteSheet, string textureMapFile)
        {
            _spriteSheet = spriteSheet;
             LoadTextureMap(textureMapFile);
            InitializeMenuLayout();

        }

        private void LoadTextureMap(string textureMapFile)
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
                            (Type)Enum.Parse(typeof(Type), data[0]), //Tile Name
                            new Rectangle( //Source Rect
                                int.Parse(data[2]) * 16, //X-pivot = ColIndex * 16 (16 is standard pixel scale, no space between tile)
                                int.Parse(data[1]) * 32, //Y-pivot = RowIndex * 32
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

        public void Draw(SpriteBatch sb)
        {
           
            if(visible)
            {
                foreach (var (buttonRect, textRect) in _menuItems)
                {

                    // draw button
                    sb.Draw(_spriteSheet, buttonRect, _textureMap[Type.LargeButton], Color.White);

                    // determine which text was intended for this button by its Y position
                    sb.Draw(_spriteSheet, textRect, _textureMap[Type.LoadText], Color.White);
                }
            }
        }

       
    }
}