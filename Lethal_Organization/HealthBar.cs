using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    class HealthBar : UI
    {
        public bool Visible { get ; set ; }

        private Texture2D _UISPriteSHeet;

        private Rectangle _healthBarSourceImg;

        private Rectangle _healthDisplayPos;

        public HealthBar(GameObject target, Texture2D UISPriteSHeet, Rectangle sourceImage, Rectangle displayPos)
        {
            target.onHealthChanged += UpdateHP;
            _UISPriteSHeet = UISPriteSHeet;
            _healthDisplayPos = displayPos;
            _healthBarSourceImg = sourceImage;
        }

        public void Draw(SpriteBatch sb)
        {
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

        public void Update(GameTime gameTime)
        {
            
        }

        public void UpdatePos(Vector2 displayPos)
        {
            _healthDisplayPos.X = (int)displayPos.X;
            _healthDisplayPos.Y = (int)displayPos.Y;
        }

        private void UpdateHP(int curHP, int maxHP)
        {
            float newWidth = (curHP / (float)maxHP) * _healthDisplayPos.Width;
            _healthDisplayPos.Width = (int)newWidth;
        }


    }
}
