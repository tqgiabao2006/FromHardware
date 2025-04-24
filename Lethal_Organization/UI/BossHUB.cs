using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Lethal_Organization.UI
{
    class BossHUB: UI
    {
        private HealthBar _healthBar;

        private SpriteFont _font;

        private Rectangle _displayPos;
        public bool Visible { get; set; }
       

        public BossHUB(Boss boss, SpriteFont font, Texture2D uiSpriteSheet, Rectangle sourceImg, int screenWidth, int screenHeight)
        {
            _displayPos = new Rectangle(screenWidth/2 - (int)(screenWidth * 0.3f), screenHeight - 200, (int)(screenWidth * 0.6f), 48);
            
            _font = font;
            _healthBar = new HealthBar(boss, uiSpriteSheet, sourceImg, _displayPos);

        }


        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(_font, "Glacior, Warden of the Frozen Realm", new Vector2(_displayPos.X,_displayPos.Y - 30), Color.Aquamarine);
            _healthBar.Draw(sb);
        }

        public void Hide()
        {
            Visible = false;    
        }

        public void Show()
        {
            Visible = true;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
           _healthBar.Update(gameTime);
        }
    }
}
