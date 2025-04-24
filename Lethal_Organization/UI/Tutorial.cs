using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization.UI
{
    class Tutorial : UI
    {
        public bool Visible { get; set; }

        private Texture2D _tutorial;

        private Rectangle _displayPos;

        private Button _exitButton;
        public Tutorial(AudioManager audioManager, Texture2D tutorialMenu, Texture2D exitButton, int screenWidth, int screenHeight)
        {
            _tutorial = tutorialMenu;

            _displayPos = new Rectangle(screenWidth / 2 - tutorialMenu.Width / 2, screenHeight / 2 - screenHeight / 2, tutorialMenu.Width, tutorialMenu.Height);

            Rectangle buttonPos = new Rectangle(_displayPos.X + _displayPos.Width - exitButton.Width, _displayPos.Y + 200, exitButton.Width, exitButton.Height);

            _exitButton = new Button(audioManager, exitButton, buttonPos, Hide);
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_tutorial, _displayPos, Color.White);
            _exitButton.Draw(sb);
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
            _exitButton.Update(gameTime);
        }
    }
}
