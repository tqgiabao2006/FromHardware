using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Lethal_Organization
{
    class HeatlhBar: UI
    {
        private Texture2D _UISPriteSHeet;

        private Rectangle _healthBarSourceImg;

        private Rectangle _borderSourceImg;

        private Rectangle _healthDisplayPos;

        private Rectangle _borderDisplayPos;

        private float HP;

        private float _maxHP;

    
        public bool Visible { get; set; }

        public HeatlhBar(Texture2D spriteSheet, Rectangle sourceImage, Rectangle placeholderSource)
        {
            _UISPriteSHeet = spriteSheet; 

            _healthBarSourceImg = sourceImage;

            _borderSourceImg = placeholderSource;

            _healthDisplayPos = new Rectangle(100, 103, sourceImage.Width * 5, sourceImage.Height * 3);

            _borderDisplayPos = new Rectangle(100, 100, placeholderSource.Width * 5, placeholderSource.Height * 3);

            HP = 100;

            _maxHP = 100;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_UISPriteSHeet, _borderDisplayPos, _borderSourceImg, Color.White);
            sb.Draw(_UISPriteSHeet, _healthDisplayPos, _healthBarSourceImg, Color.White);
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
            HP -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            float width = _healthDisplayPos.Width;
            float newWidth = (HP/_maxHP) * width;
            _healthDisplayPos.Width = (int)newWidth;
        }
    }
}
