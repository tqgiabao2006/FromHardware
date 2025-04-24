using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization.UI
{
    class YouDie : UI
    {
        public bool Visible { get ; set; }

        private Texture2D _youDie;

        private SpriteFont _font;

        private float _blinkTimer;

        private float _blinkSpeed;

        private bool _reveal;

        private Rectangle imagePos;

        private Vector2 textPos;
        public YouDie(Texture2D youDie,SpriteFont font, int screenWidth, int screenHeight)
        {
            _youDie = youDie;
            _font = font;

            _blinkTimer = 0;
            _blinkSpeed = 0.2f;
            _reveal = true;

            imagePos = new Rectangle(0,0 , screenWidth, screenHeight);

            textPos = new Vector2(screenWidth / 2 - 200, screenHeight / 2 + 200);


        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_youDie, imagePos, Color.White); 
            sb.DrawString(_font, "Click 'Enter' to respawn", textPos, Color.Red);
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
        }
    }
}
