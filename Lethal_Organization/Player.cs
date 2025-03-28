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
        private KeyboardState _currentKb;
        private KeyboardState _prevKb;
        private MouseState _mouse;
        public bool _onGround;
        public PlayerState _playerState;

        public float _rayCastLength;

        private Tile[,] _level;

        private Vector2 _playerVelocity = Vector2.Zero;
        private Vector2 _jumpVelocity = new Vector2(0, -15.0f);
        private Vector2 _gravity = new Vector2(0, 0.5f);

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
            _playerState = PlayerState.Idle;
            _rayCastLength = 40f;
            speed.X = 5;
        }

        public override void Update(GameTime gameTime)
        {
            _currentKb = Keyboard.GetState();
            _mouse = Mouse.GetState();
            Move();
            _prevKb = Keyboard.GetState();

        }

        public override void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(texture, position, Color.White);

            if (isDebug)
            {
                CustomDebug.DrawWireRectangle(sb, position, 0.5f, Color.Red);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Move()
        {
            _playerVelocity += _gravity;
            position.Y += (int)_playerVelocity.Y;

            Collisions();

            switch(_playerState)
            {
                case PlayerState.Idle:

                    if ((_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left) 
                        || _currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                        )
                    {
                        _playerState = PlayerState.Run;

                    } else if((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) || _currentKb.IsKeyDown(Keys.Space)) 
                        && _onGround)
                    {
                         
                        _playerState = PlayerState.Jump;
                        
                    } 
                    break;

                case PlayerState.Run:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                    {
                        position.X -= (int)speed.X;
                    }else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    {
                        position.X += (int)speed.X;
                    }
                    
                    
                    if (_currentKb.GetPressedKeyCount() == 0)
                    {
                        _playerState = PlayerState.Idle;
                    }else if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) ||
                               _currentKb.IsKeyDown(Keys.Space))
                              && _onGround)
                    {
                         
                        _playerState = PlayerState.Jump;
                    }
                    
                    break;
                case PlayerState.Attack:
                    
                    break;

                case PlayerState.Jump:
                    _onGround = false;
                    _playerVelocity = _jumpVelocity;
                    position.Y += (int)_playerVelocity.Y;
                    _playerState = PlayerState.Idle;
                    break;
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
        public bool OnGround()
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
                        return true;

                        //Check player stand on the collider
                        Rectangle collidedObj = this.CollisionWith(_level[i, j].PosRect);
                        if (position.Y + position.Height >= collidedObj.Y)
                        {
                            return true;
                        }
                        // //Check downward
                        // Vector2 raycastPoint = new Vector2(position.X + position.Width/2f,  position.Y + position.Height/2f + _rayCastLength);
                        //
                        // if (collidedObj.X <= raycastPoint.X
                        //     && collidedObj.X + collidedObj.Width >= raycastPoint.X
                        //     && collidedObj.Y <= raycastPoint.Y
                        //     & collidedObj.Y + collidedObj.Height >= raycastPoint.Y
                        //    )
                        // {
                        //     return true;
                        // }
                    }
                }
            }
            return false;
        }
         public void StayOnGround()
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
                         if (position.Y + position.Height >= collidedObj.Y)
                         {
                             position.Y = collidedObj.Y - position.Height;
                         }
                     }
                 }
             }
         }

        public bool Collisions()
        {
            List<Rectangle> intersectedObj = new List<Rectangle>();
            for (int i = 0; i < _level.GetLength(0); i++)
            {
                for (int j = 0; j < _level.GetLength(1); j++)
                {
                    if (_level[i, j] == null)
                    {
                        continue;
                    }
                    if (this.Collides(_level[i, j].PosRect))
                    {
                        intersectedObj.Add(_level[i, j].PosRect);
                    }
                }
            }

            


            foreach (Rectangle rec in intersectedObj)
            {
                Rectangle intersect = Rectangle.Intersect(rec, position);
                if (intersect.Height > intersect.Width)
                {
                    if (rec.X > position.X)
                    {
                        position.X -= intersect.Width;
                    }
                    else
                    {
                        position.X += intersect.Width;
                    }

                }
                else
                {
                    if (rec.Y > position.Y)
                    {
                        position.Y -= intersect.Height;
                    }
                    else
                    {

                        position.Y += intersect.Height;
                    }
                    _playerVelocity.Y = 0;
                    _onGround = true;
                    return true;
                }
            }
            return false;
        }
    }
}
