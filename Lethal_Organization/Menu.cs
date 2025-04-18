
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization
{
    internal class Menu : IDrawable
    {
        public enum Type
        {
            NewText,
            LoadText,
            OptionsText,
            ExitText,
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
            BackText, 
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

        private Type[] _types;

        private Vector2 _position;
        
        private int _scale;
        
        protected bool visible;

        protected bool isDebug;

        protected bool paused;

        private Rectangle[] _button;

        private Rectangle[] _text;

        private bool _isHovered;

        private bool _isPressed;

        private Action<GameManager.GameState> _changeState;

        public Menu(Texture2D spriteSheet, string textureMapFile, Vector2 position, GameManager gameManager, Action<GameManager.GameState> changeState ,int scale = 5)
        {
            _spriteSheet = spriteSheet;
            _position = position;
            _scale = scale;
            _textureMap = new Dictionary<Type, Rectangle>();
            _menuItems = new List<(Rectangle, Rectangle)>();
            _button = new Rectangle[4];
            _text = new Rectangle[4];
            _types = Enum.GetValues(typeof(Type)) as Type[];
            gameManager.StateChangedAction += OnStateChange;

            _changeState = changeState;

            InitializeTextureMap(textureMapFile);
            InitializeMenuLayout();
        }

        public void OnStateChange(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Menu:
                    paused = false;
                    visible = true;
                    isDebug = false;
                    break;

                case GameManager.GameState.Game:
                    visible = false;
                    paused = false;
                    isDebug = false;
                    break;

                case GameManager.GameState.GameOver:
                    visible = true;
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
                    visible = false;
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
            _button[0] = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            _text[0] = new Rectangle(
                        _button[0].X + (_button[0].Width - _textureMap[Type.NewText].Width * _scale) / 2,
                        _button[0].Y + (_button[0].Height - _textureMap[Type.NewText].Height * _scale) / 2 + 10,
                        _textureMap[Type.NewText].Width * 4,
                         _textureMap[Type.NewText].Height * 4
                    );
            _menuItems.Add((_button[0], _text[0]));

            _button[1] = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y + 100,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);
            _text[1] = new Rectangle(
                       _button[1].X + (_button[1].Width - _textureMap[Type.LoadText].Width * _scale) / 2,
                       _button[1].Y + (_button[1].Height - _textureMap[Type.LoadText].Height * _scale) / 2 + 10,
                       _textureMap[Type.NewText].Width * 4,
                        _textureMap[Type.NewText].Height * 4
                        );
            _menuItems.Add((_button[1], _text[1]));

            _button[2] = new Rectangle(
                       (int)_position.X,
                       (int)_position.Y + 200,
                       _textureMap[Type.LargeButton].Width * 4,
                       _textureMap[Type.LargeButton].Height * 4);

            _text[2] = new Rectangle(
                       _button[2].X + (_button[2].Width - _textureMap[Type.OptionsText].Width * _scale) / 2,
                       _button[2].Y + (_button[2].Height - _textureMap[Type.OptionsText].Height * _scale) / 2 + 10,
                       _textureMap[Type.OptionsText].Width * 4,
                        _textureMap[Type.OptionsText].Height * 4
                        );

            _button[3] = new Rectangle(
                        (int)_position.X,
                        (int)_position.Y + 300,
                        _textureMap[Type.LargeButton].Width * 4,
                        _textureMap[Type.LargeButton].Height * 4);

            _text[3] = new Rectangle(
                    _button[3].X + (_button[3].Width - _textureMap[Type.ExitText].Width * _scale) / 2,
                    _button[3].Y + (_button[3].Height - _textureMap[Type.ExitText].Height * _scale) / 2 + 10,
                    _textureMap[Type.ExitText].Width * 4,
                     _textureMap[Type.ExitText].Height * 4
                     );
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
            if (visible)
            {
                MouseState mouseState = Mouse.GetState();

                for (int i = 0; i < 4; i++)
                {
                        _isHovered = _button[i].Contains(mouseState.Position);
                    if (_isHovered)
                    {
                        sb.Draw(_spriteSheet, _button[i], _textureMap[Type.LargeButtonHover], Color.White);
                    }

                    if (_isHovered && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        _isPressed = true;
                        sb.Draw(_spriteSheet, _button[i], _textureMap[Type.LargeButtonPress], Color.White);
                    }

                    else if (_isHovered && mouseState.LeftButton == ButtonState.Released)
                    {
                        _isPressed = false;
                    }
                    else
                    {
                        // draw button
                        sb.Draw(_spriteSheet, _button[i], _textureMap[Type.LargeButton], Color.White);
                    }
                        // determine which text was intended for this button by its Y position
                        sb.Draw(_spriteSheet, _text[i], _textureMap[_types[i]], Color.White);

                }
                
            }
        }
    }
}