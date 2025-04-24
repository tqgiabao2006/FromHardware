
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization.UI
{
    internal class Menu : UI
    {
       
        private int _screenWidth;

        private int _screenHeight;

        private Texture2D _spriteSheet;

        private Texture2D _startGameSprite;

        private Texture2D _creditButton;

        private Texture2D _creditMenu;

        private Texture2D _tutorialMenu;

        private Texture2D _tutorialButton;

        private Texture2D _exitSprite;

        private Texture2D _openScreenSPrite;


        private Action<GameManager.GameState> _changeState;

        //New design _ Menu pos

        private Button[] _newButtons;

        private Rectangle _startPos;

        private Rectangle _tutorialPos;

        private Rectangle _creditPos;

        private Rectangle _exitPos;

        private int _menuSpacing;

        public bool Visible { get ; set ; }

        public Menu(AudioManager audioManager,Action<GameManager.GameState> changeState,
            Texture2D openTheme,  Texture2D startGame, Texture2D credit, Texture2D creditMenu, Texture2D tutorial, Texture2D tutorialMenu,
            Texture2D exit, Texture2D exitButton,
            int screenWidth, int screenHeight,
            Action showCreditMenu, Action showTutorialMenu, Action exitGame)
        {
            //Screen size
            _screenHeight = screenHeight;

            _screenWidth = screenWidth;

            //Textur 2D
            _openScreenSPrite = openTheme;

            _tutorialButton = tutorial;

            _tutorialMenu = tutorialMenu;
            
            _creditButton = credit;

            _creditMenu = creditMenu;

            _exitSprite = exit;

            _startGameSprite = startGame;

            //Calculate menu pos
            _menuSpacing = 30;

            _startPos = new Rectangle(_screenWidth / 2 - _startGameSprite.Width / 2, _screenHeight / 2, _startGameSprite.Width, _startGameSprite.Height);

           _tutorialPos = new Rectangle(_screenWidth / 2 - _tutorialButton.Width / 2, _startPos.Y + _startPos.Height + _menuSpacing, _tutorialButton.Width, _tutorialButton.Height);

            _creditPos = new Rectangle(_screenWidth / 2 - _creditButton.Width / 2, _tutorialPos.Y + _tutorialPos.Height + _menuSpacing, _creditButton.Width, _creditButton.Height);

            _exitPos = new Rectangle(_screenWidth / 2 - _exitSprite.Width / 2, _creditPos.Y + _creditPos.Height + _menuSpacing, _exitSprite.Width, _exitSprite.Height);

            _newButtons = new Button[]
            {
                new Button(audioManager, _startGameSprite, _startPos, StartGame),
                new Button(audioManager, _tutorialButton, _tutorialPos, showTutorialMenu),
                new Button(audioManager, _creditButton, _creditPos, showCreditMenu),
                new Button(audioManager, _exitSprite, _exitPos,exitGame ),
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