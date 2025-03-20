﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal abstract class GameObject : IGetHit
    {
        protected int hp;
        public int HP
        {
            get
            {
                return hp;
            }
        }

        protected Rectangle position;
        protected Rectangle sourceImg;
        protected Texture2D texture;
        protected Vector2 speed;
        protected int damage;

        public virtual void Draw(SpriteBatch sb)
        {

        }

        public abstract void Update(GameTime gameTime);

        public bool CollisionWith(GameObject other)
        {
            return position.Intersects(other.position);
        }

        public Rectangle CollisionWith(Rectangle other)
        {
            return Rectangle.Intersect(position, other);
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
