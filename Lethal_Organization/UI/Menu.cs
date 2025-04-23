
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
    internal class Menu : UI
    {
       
        private int _screenWidth;

        private int _screenHeight;

        private Texture2D _spriteSheet;

        private Texture2D _startGameSprite;

        private Texture2D _loadGameSprite;

        private Texture2D _exitSprite;

        private Texture2D _optionSprite;

        private Texture2D _openScreenSPrite;


        private Action<GameManager.GameState> _changeState;

        //New design _ Menu pos

        private Button[] _newButtons;

        private Rectangle _startPos;

        private Rectangle _loadPos;

        private Rectangle _optionPos;

        private Rectangle _exitPos;

        private int _menuSpacing;

        public bool Visible { get ; set ; }

        public Menu(Action<GameManager.GameState> changeState,
            Texture2D openTheme, Texture2D loadGame, Texture2D startGame, Texture2D exit, Texture2D option,
            int screenWidth, int screenHeight)
        {
            //Screen size
            _screenHeight = screenHeight;

            _screenWidth = screenWidth;

            // 2D
            
            _openScreenSPrite = openTheme;

            _loadGameSprite = loadGame;

            _exitSprite = exit;
            
            _optionSprite = option;

            _startGameSprite = startGame;

            //Calculate menu pos

            _menuSpacing = 30;

            _startPos = new Rectangle(_screenWidth / 2 - _startGameSprite.Width / 2, _screenHeight / 2, _startGameSprite.Width, _startGameSprite.Height);

           _loadPos = new Rectangle(_screenWidth / 2 - _loadGameSprite.Width / 2, _startPos.Y + _startPos.Height + _menuSpacing, _loadGameSprite.Width, _loadGameSprite.Height);

            _optionPos = new Rectangle(_screenWidth / 2 - _optionSprite.Width / 2, _loadPos.Y + _loadPos.Height + _menuSpacing, _optionSprite.Width, _optionSprite.Height);

            _exitPos = new Rectangle(_screenWidth / 2 - _exitSprite.Width / 2, _optionPos.Y + _optionPos.Height + _menuSpacing, _exitSprite.Width, _exitSprite.Height);


            _newButtons = new Button[]
            {
                new Button(_startGameSprite, _startPos, StartGame),
                new Button(_loadGameSprite, _loadPos, null),
                new Button(_optionSprite, _optionPos, null),
                new Button(_exitSprite, _exitPos, null),
            };


            _changeState = changeState;
            
        }

    
        public void Update(GameTime gameTime)
        {
            foreach (Button button in _newButtons)
            {
                button.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (Visible)
            {
                MouseState mouseState = Mouse.GetState();

                //Draw background_Button
                sb.Draw(_openScreenSPrite, new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);

              
                foreach(Button button in _newButtons)
                {
                    button.Draw(sb);
                }

               
            }
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }
    
        private void StartGame()
        {
            _changeState(GameManager.GameState.Game);
        }

    
    }
}