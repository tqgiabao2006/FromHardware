using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization
{
    public class Player : GameObject, IStateChange
    {
        public enum State
        {
            Idle,
            Run,
            Jump,
            Fall,
            Attack,
            Die
        }

        //Input
        private KeyboardState _currentKb;

        private KeyboardState _prevKb;
        
        private MouseState _mouse;
        
        // Movement stat:
        private Vector2 _velocity = Vector2.Zero;
        
        private float _gravity;
        
        private float _jumpForce;
        
        private Vector2 _maxSpeed;

        private bool _faceRight;

        //Player State logic
        private Rectangle hitBox;

        public State _playerState;

        //Camera && Collsion handler:
        private Vector2 _cameraOffset;

        private Level _level;

        private float _groundRayLength;

        private Vector2 _groundRayPoint;

        private Vector2 _lefRayPoint;

        private Vector2 _rightRayPoint;

        private bool _onGround;

        //Animation:
        private Dictionary<string, Rectangle> _playerSprites;

        //Shoot
        private ObjectPooling _objectPooling;
        
        private float _shootTimeCounter;

        private float _shootDelayTime;

        private bool _isShooting;

        private Texture2D _bulletTexture;

        //Animation
        Animator<Player.State> _animator;

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
        public Vector2 LeftRayPoint
        {
            get
            {
                return _lefRayPoint;
            }
        }

        public Vector2 RightRayPoint
        {
            get
            {
                return _rightRayPoint;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
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
            set
            {
                worldPos = value;
            }
        }
        public Vector2 CameraOffset
        {
            get 
            { 
                return _cameraOffset; 
            }
        }
        public Player(Texture2D playerSpriteSheet, string spriteMapFile ,Texture2D bulletTexture,GraphicsDeviceManager graphics, Level level, GameManager gameManager, ObjectPooling objectPooling)
        {            
            _bulletTexture = bulletTexture;
            
            displayPos = new Rectangle((graphics.PreferredBackBufferWidth - 75)/2, (graphics.PreferredBackBufferHeight - 48)/2,64, 48);
            
            sourceImg = new Rectangle(0, 0,64, 48);
            
            worldPos = new Rectangle(8, 384, 64, 48);
            
            _cameraOffset = new Vector2(0, 0);

            hitBox = new Rectangle(worldPos.X, worldPos.Y, 16, 48);
            
            _playerState = State.Jump;
            
            speed = 1;
            
            _maxSpeed.X = 4;
            _maxSpeed.Y = 10;
            
            _jumpForce = -10;
            
            _gravity = 0.3f;

            _groundRayLength = 30;

            _shootDelayTime = 0.4f;
            
            _shootTimeCounter = 0;

            _faceRight = true;
            
            this._level = level;

            gameManager.StateChangedAction += OnStateChange;
            
            _objectPooling = objectPooling;

            _animator = new Animator<State>(playerSpriteSheet, State.Fall, spriteMapFile, 0.1f);

            InitializePlayerSprites("playerTileMap");
        }


        public void OnStateChange(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Menu:
                    paused = false;
                    visible = false;
                    isDebug = false;
                    break;

                case GameManager.GameState.Game:
                    visible = true;
                    paused = false;
                    isDebug = false;

                    break;

                case GameManager.GameState.GameOver:
                    visible = false;
                    paused = false;
                    isDebug = false;

                    break;
                case GameManager.GameState.Pause:
                    paused = true;
                    visible = true;
                    isDebug = false;

                    break;
                case GameManager.GameState.Debug:
                    paused = false;
                    visible = true;
                    isDebug = true;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Update input
            _currentKb = Keyboard.GetState();
            _mouse = Mouse.GetState();

            //Update camera offset
            UpdateCameraOffset();

            //Animator
            _animator.Update(gameTime);

            if(visible && !paused)
            {
                //Update move logic
                StateMachine(_level);
                
                Shoot(gameTime);

                foreach(Bullet bullet in _objectPooling.Bullets)
                {
                    if(bullet.Enabled)
                    {
                        bullet.Update(gameTime);
                    }
                }
            }
          
            //Update hit box
            UpdateHitBox();

            UpdateRaycast();

            //Re-check input
            _prevKb = Keyboard.GetState();
          
        }

        public override void Draw(SpriteBatch sb)
        {
            if(visible)
            {
                SpriteEffects effect = _faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                _animator.Draw(sb, displayPos, effect);
            }

            foreach(Bullet bullet in _objectPooling.Bullets)
            {
                bullet.Draw(sb, isDebug, _cameraOffset);
            }

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
        private void StateMachine(Level level)
        {
            if (!_onGround && !isDebug)
            {
                _velocity.Y = Math.Min(_maxSpeed.Y, _velocity.Y + _gravity);
            }
           
            worldPos.X += (int)_velocity.X;

            worldPos.Y += (int)_velocity.Y;

            CollisionHandler(level);

            switch (_playerState)
            {
                case State.Idle:
                    _velocity = Vector2.Zero;

                    if ((_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left)
                        || _currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right) && 
                        _onGround)
                        )
                    {
                        _animator.SetState(State.Run);
                        _playerState = State.Run;  
                    }
                    else if(!_onGround && !isDebug)
                    {
                        _animator.SetState(State.Fall);
                        _playerState = State.Fall;
                    }

                    if (isDebug)
                    {
                        if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up)) && Math.Abs(_velocity.Y) < Math.Abs(_maxSpeed.X))
                        {
                            _playerState = State.Run;
                        }
                        else if ((_currentKb.IsKeyDown(Keys.S) || _currentKb.IsKeyDown(Keys.Down)) && Math.Abs(_velocity.Y) < Math.Abs(_maxSpeed.X))
                        {

                            _playerState = State.Run;
                        }
                    }
                    else if ((IsSinglePressed(Keys.W) || IsSinglePressed(Keys.Up) || IsSinglePressed(Keys.Space))
                            && _onGround && !isDebug)
                    {
                        _animator.SetState(State.Jump);
                        _playerState = State.Jump;
                        _velocity.Y += _jumpForce;
                    }

                    break;

                case State.Run:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left) && _onGround)
                    {
                        if (_velocity.X > -_maxSpeed.X)
                        {
                            _velocity.X -= speed;
                        }

                        _faceRight = false;
                    
                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right) && _onGround)
                    {
                        if (_velocity.X < _maxSpeed.X)
                        {
                            _velocity.X += speed;
                        }

                        _faceRight = true;
                    }
                    else if (!_onGround && !isDebug)
                    {
                        _animator.SetState(State.Fall);
                        _playerState = State.Fall;
                    }


                    if (_currentKb.GetPressedKeyCount() == 0)
                    {
                        _animator.SetState(State.Idle);
                        _playerState = State.Idle;
                    }

                    if (isDebug)
                    {
                        if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up)) && Math.Abs(_velocity.Y) < Math.Abs(_maxSpeed.X))
                        {
                            _velocity.Y -= speed;
                        }
                        else if ((_currentKb.IsKeyDown(Keys.S) || _currentKb.IsKeyDown(Keys.Down)) && Math.Abs(_velocity.Y) < Math.Abs(_maxSpeed.X))
                        {
                            _velocity.Y += speed;
                        }
                    }
                    else if ((IsSinglePressed(Keys.W) || IsSinglePressed(Keys.Up) || IsSinglePressed(Keys.Space))
                            && _onGround && !isDebug)
                    {
                        _animator.SetState(State.Jump);
                        _playerState = State.Jump;
                        _velocity.Y += _jumpForce;
                    }

                    break;

                case State.Attack:

                    break;

                case State.Jump:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                    {
                        if (_velocity.X > -_maxSpeed.X)
                        {
                            _velocity.X -= speed;
                        }

                        _faceRight = false;
                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    {
                        if (_velocity.X < _maxSpeed.X)
                        {
                            _velocity.X += speed;
                        }

                        _faceRight = true;
                    }


                    if (_velocity.Y >= 0 && !isDebug)
                    {
                        _animator.SetState(State.Fall);
                        _playerState = State.Fall;
                    }
                    
                    break;

                case State.Fall:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                    {
                        if (_velocity.X > -_maxSpeed.X)
                        {
                            _velocity.X -= speed;
                        }

                    }
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                    {
                        if (_velocity.X < _maxSpeed.X)
                        {
                            _velocity.X += speed;
                        }
                    }

                    if (_onGround && _velocity.Y >= 0)
                    {
                        _animator.SetState(State.Idle);
                        _playerState = State.Idle;
                    }
                    break;
          
            }

        }

        private void Shoot(GameTime gameTime)
        {
            _shootTimeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_mouse.LeftButton == ButtonState.Pressed)
            {
                _isShooting = true;
              
                if (_shootTimeCounter <= 0)
                {
                    Bullet bullet = _objectPooling.GetObj(ObjectPooling.ProjectileType.Bullet, _bulletTexture, _level);
                    Bullet.Direction direction = _faceRight ? Bullet.Direction.Right : Bullet.Direction.Left;
                    int dirMultipler = _faceRight ? 1 : -1;
                    Vector2 spawnPos = new Vector2(this.worldPos.Center.X + dirMultipler * this.worldPos.Width / 2, this.worldPos.Center.Y);
                    _animator.SetState(State.Attack);
                    bullet.Spawn(spawnPos, direction);
                    _shootTimeCounter = _shootDelayTime;
                }
                
            }
            else if(_mouse.RightButton == ButtonState.Released)
            {
                _animator.SetState(_playerState);
                _isShooting = false;
            }
        }
        
        public void CollisionHandler(Level level)
        {
            bool groundRayHit = false;
            bool collided = false;
            for (int i = 0; i < level.SizeX; i++)
            {
                for (int j = 0; j < level.SizeY; j++)
                {
                    //Check collision
                    if (level[i, j] == null)
                    {
                        continue;
                    }

                    Rectangle tilePos = level[i, j].WorldPos;

                    //Check on ground
                    if ((IsInside(tilePos, _groundRayPoint) || IsInside(tilePos, _lefRayPoint) || IsInside(tilePos, _rightRayPoint)) //Check ray point
                        && !groundRayHit
                        && worldPos.Y < tilePos.Y && !isDebug) //Player on the top of the tile
                     
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


                    if (this.Collides(hitBox, tilePos) && !isDebug)
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
                        if (hitBox.Y > tilePos.Y && collidedArea.Width > collidedArea.Height)// hit box under the tile
                                                                                             //hitBox.X > tilePos.X && hitBox.X < tilePos.X + tilePos.Width// hit box land between the left and right of a tile


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
            _lefRayPoint = new Vector2(hitBox.X, hitBox.Y + hitBox.Height / 2 + _groundRayLength);
            _rightRayPoint = new Vector2(hitBox.X + hitBox.Width, hitBox.Y + hitBox.Height / 2 + _groundRayLength);
        }

        /// <summary>
        /// Set camera offset
        /// </summary>
        private void UpdateCameraOffset()
        {
            _cameraOffset.X = displayPos.X - worldPos.X;
            _cameraOffset.Y = displayPos.Y - worldPos.Y;
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
