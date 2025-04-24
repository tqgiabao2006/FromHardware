using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization.UI
{
    class Setting : UI
    {
        public bool Visible { get; set ; }

        private Texture2D _uiSpriteSheet;

        private Rectangle _homeIcon;

        private Rectangle _pauseIcon;

        private Rectangle _hover;

        private Rectangle _normal;

        private Rectangle _pressed;

        private Button _paused;
        
        private Button _home;

        private Rectangle _pausePos;

        private Rectangle _homePos;

        private Action<GameManager.GameState> _changeState;
        public Setting(AudioManager audioManager, Action<GameManager.GameState> changState,  Texture2D UISpriteSheet, 
            Rectangle home, Rectangle pause, Rectangle smallButton, Rectangle pressButton, Rectangle hoverButton, 
            int screenWidth, int screenHeight)
        {

            _changeState = changState;

            _uiSpriteSheet = UISpriteSheet;

            _homeIcon = home;

            _pauseIcon = pause;

            _normal = smallButton;

            _pressed = pressButton;

            _hover = hoverButton;   
        
            _homePos = new Rectangle(screenWidth - smallButton.Width*4,10, smallButton.Width*4, smallButton.Height*4);

            _pausePos = new Rectangle(_homePos.X - smallButton.Width*4 - 10, 10, smallButton.Width * 4, smallButton.Height * 4);

            _paused = new Button(audioManager,_uiSpriteSheet, _pausePos, Pause, UnPause);

            _home = new Button(audioManager,_uiSpriteSheet, _homePos, OpenHome);

        }

        public void Draw(SpriteBatch sb)
        {
            _home.Draw(sb, _pressed, _hover, _normal);

            _paused.Draw(sb, _pressed, _hover, _normal);

            sb.Draw(_uiSpriteSheet, _homePos, _homeIcon, Color.White);

            sb.Draw(_uiSpriteSheet, _pausePos, _pauseIcon, Color.White);

        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public void Update(GameTime gameTime)
        {
            _paused.Update(gameTime);   
            _home.Update(gameTime);
        }

        private void OpenHome()
        {
            _changeState(GameManager.GameState.Menu);
        }

        private void Pause()
        {
            _changeState(GameManager.GameState.Pause);
        }

        private void UnPause()
        {
            _changeState(GameManager.GameState.Game);
        }
    }
}
