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

        public Rectangle Position
        {
            get { return position; }
        }

        public override void Update(GameTime gameTime)
        {
            currentKb = Keyboard.GetState();
            Move();
            prevKb = Keyboard.GetState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Move()
        {
            if (currentKb.IsKeyDown(Keys.A) || currentKb.IsKeyDown(Keys.Left))
            {
                position.X -= (int)speed.X;
            }
            if (currentKb.IsKeyDown(Keys.D) || currentKb.IsKeyDown(Keys.Right))
            {
                position.X += (int)speed.X;
            }
            if ((currentKb.IsKeyDown(Keys.W) || currentKb.IsKeyDown(Keys.Up) || currentKb.IsKeyDown(Keys.Space)) && !airborne)
            {
                position.Y -= (int)speed.Y;
                playerState = PlayerState.Jump;
            }
        }

        private void Jump()
        {

        }

        private void Attack()
        {

        }

        private void SpecialAttack()
        {

        }
    }
}
