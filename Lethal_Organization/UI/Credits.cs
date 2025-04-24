using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization.UI
{
    internal class Credits:UI
    {
        public bool Visible { get ; set ; }

        private Texture2D _creditMenu;

        private Rectangle _displayPos;

        private Button _exitButton;
        public Credits(AudioManager audioManager, Texture2D creditMenu, Texture2D exitButton, int screenWidth, int screenHeight)
        {
            _creditMenu = creditMenu;

           _displayPos = new Rectangle(screenWidth / 2 - creditMenu.Width / 2, screenHeight / 2 - screenHeight / 2, creditMenu.Width, creditMenu.Height);

            Rectangle buttonPos = new Rectangle(_displayPos.Width - exitButton.Width, _displayPos.Y + 200, exitButton.Width, exitButton.Height);

            _exitButton = new Button(audioManager, exitButton, buttonPos, Hide);
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_creditMenu, _displayPos, Color.White);
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
