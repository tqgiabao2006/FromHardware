using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Attack,
    }

    internal class Player : GameObject
    {
        //Fields
        private KeyboardState currentKb;
        private KeyboardState prevKb;
        private MouseState mouse;
        private PlayerState playerState;
        private bool airborne;

        /// <summary>
        /// Determines whether the player
        /// is airborne and whether they 
        /// can jump currently
        /// </summary>
        public bool Airborne
        {
            get { return airborne; }
            set { airborne = value; }
        }

        /// <summary>
        /// Read only position property for use with enemy patrol
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        public Player(Texture2D sprite)
        {
            texture = sprite;
            position = new Rectangle(0, 0, 75, 48);
            sourceImg = new Rectangle(0, 0, 75, 48);
            airborne = true;
        }

        public override void Update(GameTime gameTime)
        {
            currentKb = Keyboard.GetState();
            mouse = Mouse.GetState();
            Move();
            prevKb = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Move()
        {
            if (currentKb.IsKeyDown(Keys.A) || currentKb.IsKeyDown(Keys.Left))
            {
                position.X -= (int)speed.X;
                playerState = PlayerState.Run;
            }
            if (currentKb.IsKeyDown(Keys.D) || currentKb.IsKeyDown(Keys.Right))
            {
                position.X += (int)speed.X;
                playerState = PlayerState.Run;
            }
            if ((currentKb.IsKeyDown(Keys.W) || currentKb.IsKeyDown(Keys.Up) || currentKb.IsKeyDown(Keys.Space)) && !airborne)
            {
                position.Y -= (int)speed.Y;
                playerState = PlayerState.Jump;
            }
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Attack();
                playerState = PlayerState.Attack;
            }
            if (airborne)
            {
                position.Y -= (int)speed.Y / 5;
            }
            if (currentKb.GetPressedKeyCount() == 0 && !airborne)
            {
                playerState = PlayerState.Idle;
            }
        }

        /// <summary>
        /// Jump relevant logic and animation
        /// </summary>
        private void Jump()
        {

        }

        /// <summary>
        /// Attack relevant logic and animation
        /// </summary>
        private void Attack()
        {

        }

        /// <summary>
        /// NOt sure what this is supposed to be
        /// </summary>
        private void SpecialAttack()
        {

        }
    }
}
