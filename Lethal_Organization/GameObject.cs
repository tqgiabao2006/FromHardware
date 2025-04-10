using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    internal abstract class GameObject : IGetHit, IDrawable, IUpdateable
    {
        // Asset
        protected Texture2D texture;

        //Positions
        protected Rectangle displayPos; //For display only
          
        protected Rectangle sourceImg; //For render from sprite sheet
        
        protected Rectangle worldPos; //Real position to do with logic

        //Basic stats        
        protected float speed;
        
        protected int damage;

        protected int hp;
        public int HP
        {
            get
            {
                return hp;
            }
        }
        public virtual void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(
            texture,
            displayPos,
            sourceImg,
            Color.White,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
            );
        }

        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Check if this object collide with other object
        /// </summary>
        /// <param name="other">World position</param>
        /// <returns></returns>
        public bool Collides (Rectangle other)
        {
            return worldPos.Intersects(other);
        }

        /// <summary>
        /// Check if collides with hit box
        /// </summary>
        /// <param name="hitBox"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Collides(Rectangle hitBox,Rectangle other)
        {
            return hitBox.Intersects(other);
        }

        /// <summary>
        /// Check if this object is on head other object
        /// </summary>
        /// <param name="other">World position</param>
        /// <returns></returns>
        public bool OnHead(Rectangle other)
        {
            return  other.Y - worldPos.Y  < 2 && worldPos.Y + worldPos.Height <= other.Y;
        }

        /// <summary>
        /// Check if hit box is on head other object
        /// </summary>
        /// <param name="other">World position</param>
        /// <returns></returns>
        public bool OnHead(Rectangle hitBox, Rectangle other)
        {
            return other.Y - hitBox.Y < 2 && hitBox.Y + hitBox.Height <= other.Y;
        }


        /// <summary>
        /// Check if this object collide with other object 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Rectangle of collided part</returns>
        public Rectangle Collide(Rectangle other)
        {
            return Rectangle.Intersect(worldPos, other);
        }

        /// <summary>
        /// Check if this hit box collide with other object 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Rectangle of collided part</returns>
        public Rectangle Collide(Rectangle hitBox, Rectangle other)
        {
            return Rectangle.Intersect(hitBox, other);
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
