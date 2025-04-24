using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization.UI
{
    class EndScreen:UI
    {
        private int _screenWidth;

        private int _screenHeight;

        private Texture2D _endingScreen;

        private Texture2D _againTexture;

        public bool Visible { get; set; }

        private Button _againButton;

        private Action<GameManager.GameState> _changeState;

        public EndScreen(AudioManager audioManager, Action<GameManager.GameState> changeState, Texture2D endScreen, Texture2D againTexture, int screenWidth, int screenHeight)
        {
            _changeState = changeState;

            _endingScreen = endScreen;
            
            _screenHeight = screenHeight;

            _screenWidth = screenWidth;

            _againButton = new Button(audioManager, againTexture, new Rectangle(screenWidth/2 - againTexture.Width/2, screenHeight/ 2 - againTexture.Height/2 + 100, againTexture.Width, againTexture.Height), Again);
        }

      

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_endingScreen, new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);
            _againButton.Draw(sb);
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
            _againButton.Update(gameTime);
        }

        private void Again()
        {
            _changeState(GameManager.GameState.Menu);
        }
    }
}
