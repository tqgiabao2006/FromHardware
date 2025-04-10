using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization
{
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Attack,
    }

    internal class Player : GameObject
    {
        //Input
        private KeyboardState _currentKb;

        private KeyboardState _prevKb;
        
        private MouseState _mouse;
        
        // Movement stat:
        private Vector2 _velocity = Vector2.Zero;
        
        private float _gravity;
        
        private float _jumpForce;
        
        private int _maxSpeed;

        //Player State logic
        private Rectangle hitBox;

        public PlayerState _playerState;

        //Camera && Collsion handler:
        private Vector2 _cameraOffset;

        private Level _level;

        private float _groundRayLength;

        private Vector2 _groundRayPoint;

        private bool _onGround;

        //Animation:
        private Dictionary<string, Rectangle> _playerSprites;


        public bool OnGround
        {
            get { return _onGround; }
        }
        public Vector2 GroundCheckPoint
        {
            get
            {
                return _groundRayPoint;
            }
        }

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
        public Rectangle CameraPos
        {
            get 
            { 
                return displayPos; 
            }
        }
        public Rectangle WorldPos
        {
            get 
            { 
                return worldPos; 
            }
        }
        public Vector2 CameraOffset
        {
            get 
            { 
                return _cameraOffset; 
            }
        }


        public Player(Texture2D sprite,  GraphicsDeviceManager graphics, Level level)
        {
            texture = sprite;
            
            displayPos = new Rectangle((graphics.PreferredBackBufferWidth - 75)/2, (graphics.PreferredBackBufferHeight - 48)/2,64, 48);
            
            sourceImg = new Rectangle(0, 0,64, 48);
            
            worldPos = new Rectangle(0, 0, 64, 48);
            
            _cameraOffset = new Vector2(0, 0);

            hitBox = new Rectangle(worldPos.X, worldPos.Y, 16, 48);
            
            _playerState = PlayerState.Jump;
            
            speed = 1;
            
            _maxSpeed = 4;
            
            _jumpForce = -10;
            
            _gravity = 0.3f;

            _groundRayLength = 30;
            
            this._level = level;

            InitializePlayerSprites("playerTileMap");
           
        }

        public void Update(GameTime gameTime, Tile[,] tile)
        {
            //Update input
            _currentKb = Keyboard.GetState();
            _mouse = Mouse.GetState();

            //Update camera offset
            UpdateCameraOffset();

            //Update move logic
            Move(tile);

            //Update hit box
            UpdateHitBox();

            UpdateRaycast();

            //Re-check input
            _prevKb = Keyboard.GetState();
          
        }

        public override void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(texture, displayPos, Color.White);

            if (isDebug)
            {
                CustomDebug.DrawWireRectangle(sb,displayPos, 1f, Color.Aqua);
                CustomDebug.DrawWireRectangle(sb, worldPos, 1f, Color.Aqua);
                CustomDebug.DrawWireRectangle(sb, hitBox, 1f, Color.DarkOliveGreen);
            }
        }

        /// <summary>
        /// Movement logic for player 
        /// </summary>
        private void Move(Tile[,] tile)
        {
            if (!_onGround)
            {
                _velocity.Y += _gravity;
            }
           
            worldPos.X += (int)_velocity.X;

            worldPos.Y += (int)_velocity.Y;

            CollisionHandler(tile);

            switch (_playerState)
            {
                case PlayerState.Idle:
                    _velocity = Vector2.Zero;

                    if ((_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left)
                        || _currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right) && 
                        _onGround)
                        )
                    {
                        _playerState = PlayerState.Run;

                    }
                    else if ((IsSinglePressed(Keys.W) || IsSinglePressed(Keys.Up) || IsSinglePressed(Keys.Space))
                        && _onGround)
                    {
                        _playerState = PlayerState.Jump;
                        _velocity.Y += _jumpForce;

                    }else if(!_onGround)
                    {
                        _playerState = PlayerState.Fall;
                    }

                    break;

                case PlayerState.Run:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left) && _onGround)
                    {
                        if (_velocity.X > -_maxSpeed)
                        {
                            _velocity.X -= speed;
                        }
                       
                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right) && _onGround)
                    {
                        if (_velocity.X < _maxSpeed)
                        {
                            _velocity.X += speed;
                        }
                        
                    }
                    else if (!_onGround)
                    {
                        _playerState = PlayerState.Fall;
                    }


                    if (_currentKb.GetPressedKeyCount() == 0)
                    {
                        _playerState = PlayerState.Idle;
                    }
                    else if ((IsSinglePressed(Keys.W) || IsSinglePressed(Keys.Up) || IsSinglePressed(Keys.Space))
                        && _onGround)
                    {
                        _playerState = PlayerState.Jump;
                        _velocity.Y += _jumpForce;
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

                   
                    if (_velocity.Y >= 0)
                    {
                        _playerState = PlayerState.Fall;
                    }
                    
                    break;

                case PlayerState.Fall:
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

                    if (_onGround && _velocity.Y >= 0)
                    {
                        _playerState = PlayerState.Idle;
                    }
                    break;
            }

        }

        /// <summary>
        /// Jump relevant  animation
        /// </summary>
        private void Jump()
        {

        }

        /// <summary>
        /// Attack relevant animation
        /// </summary>
        private void Attack()
        {

        }


        public void CollisionHandler(Tile[,] _level)
        {
            bool groundRayHit = false;
            bool collided = false;
            for (int i = 0; i < _level.GetLength(0); i++)
            {
                for (int j = 0; j < _level.GetLength(1); j++)
                {
                    //Check collision
                    if (_level[i, j] == null)
                    {
                        continue;
                    }

                    Rectangle tilePos = _level[i, j].WorldPos;
                    
                    //Check on ground
                    if (IsInside(tilePos, _groundRayPoint) && !groundRayHit)
                    {
                        Rectangle collidedArea = this.Collide(hitBox, tilePos);
                        _onGround = true;
                        groundRayHit = true;
                        worldPos.Y -= collidedArea.Height;
                    }
                    else if(!groundRayHit)
                    {
                        _onGround = false;
                    }


                    if (this.Collides(hitBox, tilePos))
                    {
                        Rectangle collidedArea = this.Collide(hitBox, tilePos);

                        //Check horizontal collsion
                        if (collidedArea.Width < collidedArea.Height)
                        {
                            //Dispose horizontal velocity
                            if (collidedArea.X > hitBox.X)
                            {
                                worldPos.X -= collidedArea.Width;
                            }
                            else
                            {
                                worldPos.X += collidedArea.Width;
                            }

                            _velocity.X = 0;
                        }

                        //Check if hit object over-head
                        if (hitBox.Y > tilePos.Y &&                                             // hit box under the tile
                            hitBox.X > tilePos.X && hitBox.X < tilePos.X + tilePos.Width)      // hit box land between the left and right of a tile

                        {
                            //Dispose vertical velocity
                            _velocity.Y = 0;

                            worldPos.Y += collidedArea.Height;
                        }
                    }
                }
            }
            
        }


        /// <summary>
        /// Read and save each sprite to dictionary as a rectangle 
        /// </summary>
        /// <param name="playerSpritesFile"></param>
        private void InitializePlayerSprites(string playerSpritesFile)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(playerSpritesFile);
                string line = "";

                int currentWidth = 0;
                int currentHeight = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    //Skip the line start by '/' or '=' because it is description, not data
                    if (line[0] == '/' || line[0] == '=')
                    {
                        continue;
                    }

                    string[] data = line.Split(',');

                    //If it is size data
                    if (data.Length == 2)
                    {
                        currentWidth = int.Parse(data[0]);
                        currentHeight = int.Parse(data[1]);
                    }
                    else if (data.Length == 3)
                    {
                        _playerSprites.Add(
                            data[0], //Tile Name
                            new Rectangle( //Source Rect
                                int.Parse(data[2]) * 16, //X-pivot = ColIndex * 16 (16 is standard pixel scale, no space between tile)
                                int.Parse(data[1]) * 16, //Y-pivot = RowIndex * 16 
                                currentWidth,
                                currentHeight)
                        );
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("ERROR: Can not find the text file!");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Currently useless
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
           
        }

        /// <summary>
        /// Update hitbox with worldPos
        /// </summary>
        private void UpdateHitBox()
        {
            hitBox.X = worldPos.X + worldPos.Width / 2 - hitBox.Width / 2; //Align to middle of the world pos rectangle
            hitBox.Y = worldPos.Y;
        }
        
        /// <summary>
        /// Update raycast
        /// </summary>
        private void UpdateRaycast()
        {
            _groundRayPoint = new Vector2(hitBox.X + hitBox.Width / 2, hitBox.Y + hitBox.Height / 2 + _groundRayLength);
        }


        /// <summary>
        /// Set camera offset
        /// </summary>
        private void UpdateCameraOffset()
        {
            _cameraOffset.X = displayPos.X - worldPos.X;
            _cameraOffset.Y = displayPos.Y - worldPos.Y;
            _level.Offset = _cameraOffset;
        }


        /// <summary>
        /// Check if a key is single pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsSinglePressed(Keys key)
        {
            return (_currentKb.IsKeyDown(key) && _prevKb.IsKeyUp(key));
        }


        /// <summary>
        /// Check if a point is inside the box
        /// </summary>
        /// <param name="box"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsInside(Rectangle box, Vector2 point)
        {
            return point.X > box.X 
                && point.X < box.X + box.Width 
                && point.Y > box.Y 
                && point.Y < box.Y + box.Height;
        }

        /// <summary>
        /// Check if 2 float approximately equal in some threshold
        /// </summary>
        /// <returns></returns>
        private bool ApproximatelyEqual(float a, float b, float threshold)
        {
            return a > b - threshold && a < b + threshold;
        }
    }
}
