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

        private Vector2 _velocity = Vector2.Zero;
        private float _gravity;
        private float _jumpForce;
        private int _maxSpeed;

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
        }
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
            _playerState = PlayerState.Jump;
            _rayCastLength = 40f;
            speed = 1;
            _maxSpeed = 4;
            _jumpForce = -30;
            _gravity = 2;
           
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

            if (_onGround)
            {
                _velocity.Y = 0;
            }
           _velocity.Y += _gravity;

            position.X += (int)_velocity.X;

            position.Y += (int)_velocity.Y;


            StayOnGround();

            switch (_playerState)
            {
                case PlayerState.Idle:

                _velocity = Vector2.Zero;

                if ((_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left)
                    || _currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    )
                {
                    _playerState = PlayerState.Run;

                }
                else if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) || _currentKb.IsKeyDown(Keys.Space))
                    && _onGround)
                {

                    _playerState = PlayerState.Jump;
                    _velocity.Y += _jumpForce;
                    _onGround = false;
                }else if(!_onGround)
                    {
                        _playerState = PlayerState.Jump;
                    }


                break;

                case PlayerState.Run:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                    {
                        if (_velocity.X > -_maxSpeed)
                        {
                            _velocity.X -= speed;
                        }
                       
                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    {
                        if (_velocity.X < _maxSpeed)
                        {
                            _velocity.X += speed;
                        }
                    }


                    if (_currentKb.GetPressedKeyCount() == 0)
                    {
                        _playerState = PlayerState.Idle;
                    }
                    else if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) ||
                               _currentKb.IsKeyDown(Keys.Space))
                              && _onGround)
                    {
                        _playerState = PlayerState.Jump;
                        _velocity.Y += _jumpForce;
                        _onGround = false;
                    }else if (!_onGround)
                    {
                        _playerState = PlayerState.Jump;
                    }

                    break;
                case PlayerState.Attack:

                    break;

                case PlayerState.Jump:

                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                    {
                        if (_velocity.X > -_maxSpeed)
                        {
                            _velocity.X -= speed;
                        }

                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    {
                        if (_velocity.X < _maxSpeed)
                        {
                            _velocity.X += speed;
                        }
                    }


                    if (_onGround)
                    {
                        _playerState = PlayerState.Idle;
                    }                
                    
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
        public void StayOnGround()
        {
            bool hasCollided = false;
            for (int i = 0; i < _level.GetLength(0); i++)
            {
                for (int j = 0; j < _level.GetLength(1); j++)
                {
                    //Check collision
                    if (_level[i, j] == null)
                    {
                        continue;
                    }
                    if (this.Collides(_level[i, j].PosRect))
                    {
                        //Check player stand on the collider
                        Rectangle collidedObj = this.CollisionWith(_level[i, j].PosRect);
                        if (collidedObj.Width < collidedObj.Height && position.Y > _level[i, j].PosRect.Y)
                        {
                            if (collidedObj.X > position.X)
                            {
                                position.X -= collidedObj.Width;
                            }
                            else
                            {
                                position.X += collidedObj.Width;
                            }
                        }
                        
                      else if (position.Y + position.Height >= collidedObj.Y && 
                            (position.X + position.Width / 2 > _level[i, j].PosRect.X && 
                            position.X + position.Width / 2 < _level[i, j].PosRect.X + _level[i, j].PosRect.Width))
                        {
                            position.Y -= collidedObj.Height;
                            _onGround = true;
                            hasCollided = true;
                            return;
                        }
                    }
                }
            }

            _onGround = hasCollided;
        }

    }
}
