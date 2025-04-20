using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Lethal_Organization
{
    class PlayerHUB: UI
    {
        private Texture2D _UISPriteSHeet;

        private Rectangle _healthBarSourceImg;

        private Rectangle _healthDisplayPos;

        private Rectangle _borderSourceImg;

        private Rectangle _borderDisplayPos;

        private Rectangle _playerIconSourceImg;

        private Rectangle _playerIconPos;

        private float HP;

        private float _maxHP;

    
        public bool Visible { get; set; }

        public PlayerHUB(Player player, Texture2D spriteSheet, Rectangle healthBarSource, Rectangle placeholderSource, Rectangle playerIconSource)
        {
            player.onHealthChanged += UpdateHP;

            _UISPriteSHeet = spriteSheet; 

            _healthBarSourceImg = healthBarSource;

            _borderSourceImg = placeholderSource;

            _playerIconSourceImg = playerIconSource;

            _healthDisplayPos = new Rectangle(110, 153, healthBarSource.Width * 10, healthBarSource.Height * 4);

            _borderDisplayPos = new Rectangle(100, 150, placeholderSource.Width * 10, placeholderSource.Height * 4);

            _playerIconPos = new Rectangle(100, _healthDisplayPos.Y - _healthDisplayPos.Height - 30, _playerIconSourceImg.Width * 4, _playerIconSourceImg.Height * 4);

            HP = 100;

            _maxHP = 100;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_UISPriteSHeet, _borderDisplayPos, _borderSourceImg, Color.White);
            sb.Draw(_UISPriteSHeet, _healthDisplayPos, _healthBarSourceImg, Color.White);
            sb.Draw(_UISPriteSHeet, _playerIconPos, _playerIconSourceImg, Color.White);
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

        private void UpdateHP(int curHP, int maxHP)
        {
            float newWidth = (HP / _maxHP) * _healthDisplayPos.Width;
            _healthDisplayPos.Width = (int)newWidth;
        }
    }
}
