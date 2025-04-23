using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    public abstract class GameObject : IGetHit, IDrawable, IUpdateable
    {
        // Asset
        protected Texture2D texture;

        //Positions
        protected Rectangle displayPos; //For display only
          
        protected Rectangle sourceImg; //For render from sprite sheet
        
        protected Rectangle worldPos; //Real position to do with logic

        //Basic stats
        protected bool enabled;

        protected float speed;
        
        protected int damage;

        protected bool visible;

        protected bool isDebug;

        protected bool paused;

        //HP
        public delegate void OnHealthChanged(int currentHP, int maxHP);

        public event OnHealthChanged onHealthChanged;

        protected int curHP;
        public int CurHP
        {
            get
            {
                return curHP;
            }
        }

        protected int maxHp;

        public bool Visible
        {
            get { return visible; }
        }

        public bool Enabled
        {
            get { return enabled; }
        }

        public GameObject()
        {
            enabled = true; 
        }


        public virtual void Draw(SpriteBatch sb)
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

        public virtual void GetHit(int damage)
        {
            curHP =  MathHelper.Clamp(curHP - damage, 0, maxHp);
            onHealthChanged?.Invoke(curHP, maxHp);

            if(curHP <= 0)
            {
                SetActive(false);   
            }
        }

        public void SetActive(bool enabled)
        {
           this.enabled = enabled;
           visible = enabled;
        }

        protected void HPChanged(int curHp)
        {
            onHealthChanged?.Invoke(curHp, maxHp);
        }

    }
}
