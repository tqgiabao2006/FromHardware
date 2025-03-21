﻿using Microsoft.Xna.Framework;
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

        private Tile[,] _level;

        /// <summary>
        /// Read only position property for use with enemy patrol
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        public Player(Texture2D sprite, Tile[,] tile)
        {
            texture = sprite;
            position = new Rectangle(0, 0, 75, 48);
            sourceImg = new Rectangle(0, 0, 75, 48);
            _level = tile;
            speed = new Vector2(10, 3);
        }

        public override void Update(GameTime gameTime)
        {
            currentKb = Keyboard.GetState();
            mouse = Mouse.GetState();
            Move();
            prevKb = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(texture, position, Color.White);
            CustomDebug.DrawRectOutline(sb, position, 3, Color.Blue);
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
                this.position.X += (int)speed.X;
                playerState = PlayerState.Run;
            }
            if ((currentKb.IsKeyDown(Keys.W) || currentKb.IsKeyDown(Keys.Up) || currentKb.IsKeyDown(Keys.Space)) && OnGround())
            {
                position.Y -= (int)speed.Y;
                playerState = PlayerState.Jump;
            }
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Attack();
                playerState = PlayerState.Attack;
            }
            if (!OnGround())
            {
                position.Y += (int)speed.Y / 2;
            }
            if (currentKb.GetPressedKeyCount() == 0 && OnGround())
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

        private bool OnGround()
        {
            for(int i= 0; i < _level.GetLength(0); i++)
            {
                for(int j = 0; j < _level.GetLength(1); j++)
                {
                    //Check collision
                    if (_level[i,j] == null)
                    {
                        continue;
                    }
                    if (this.Collides(_level[i,j].PosRect))
                    {
                        //Check player stand on the collider
                        Rectangle collidedObj = this.CollisionWith(_level[i, j].PosRect);
                        if (this.position.Y > collidedObj.Y)
                        {
                            this.position.Y += collidedObj.Y;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
