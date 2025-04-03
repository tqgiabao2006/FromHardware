using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal abstract class GameObject : IGetHit, IDrawable, IUpdateable
    {
        protected int hp;
        public int HP
        {
            get
            {
                return hp;
            }
        }

        protected Rectangle cameraPos;
        protected Rectangle sourceImg;
        protected Texture2D texture;
        protected float speed;
        protected int damage;

        public virtual void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(
            texture,
            cameraPos,
            sourceImg,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
            );
        }

        public abstract void Update(GameTime gameTime);

        public bool Collides (Rectangle other)
        {
            return cameraPos.Intersects(other);
        }

        public bool OnHead(Rectangle screenPos)
        {
            return  screenPos.Y - cameraPos.Y  < 2 && cameraPos.Y + cameraPos.Height <= screenPos.Y;
        }

        public Rectangle CollisionWith(Rectangle screenPos)
        {
            return Rectangle.Intersect(cameraPos, screenPos);
        }

        protected void DealDamage(IGetHit iCanGetHit)
        {

        }

        public void GetHit(int damage)
        {
            hp -= damage;
        }

    }
}
