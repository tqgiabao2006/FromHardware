using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Lethal_Organization
{
    class PlayerHUB: UI
    {
        private HealthBar _healthBar;

        private Texture2D _UISPriteSHeet;

        private Rectangle _borderSourceImg;

        private Rectangle _borderDisplayPos;

        private Rectangle _playerIconSourceImg;

        private Rectangle _playerIconPos;


        public bool Visible { get; set; }

        public PlayerHUB(Player player, Texture2D spriteSheet,Rectangle healthBarSource, Rectangle placeholderSource, Rectangle playerIconSource)
        {
            _UISPriteSHeet = spriteSheet; 

            _borderSourceImg = placeholderSource;

            _playerIconSourceImg = playerIconSource;

            Rectangle healthDisplayPos = new Rectangle(60, 103, healthBarSource.Width * 10, healthBarSource.Height * 4);

            _borderDisplayPos = new Rectangle(50, 100, placeholderSource.Width * 10, placeholderSource.Height * 4);

            _healthBar = new HealthBar(player, spriteSheet, healthBarSource, healthDisplayPos);

            _playerIconPos = new Rectangle(_borderDisplayPos.X, healthDisplayPos.Y - healthDisplayPos.Height - 30, _playerIconSourceImg.Width * 4, _playerIconSourceImg.Height * 4);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_UISPriteSHeet, _borderDisplayPos, _borderSourceImg, Color.White);
            sb.Draw(_UISPriteSHeet, _playerIconPos, _playerIconSourceImg, Color.White);
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
         
        }


    }
}
