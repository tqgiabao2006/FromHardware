using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
        private Rectangle worldPos;
        private Vector2 cameraOffset;
        private Dictionary<string, Rectangle> _playerSprites;

        public float _rayCastLength;


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
        public Rectangle WorldPos
        {
            get { return worldPos; }
        }
        public Vector2 CameraOffset
        {
            get { return cameraOffset; }
        }

        public Player(Texture2D sprite,  GraphicsDeviceManager graphics)
        {
            texture = sprite;
            position = new Rectangle((graphics.PreferredBackBufferWidth - 75)/2, (graphics.PreferredBackBufferHeight - 48)/2, 75, 48);
            sourceImg = new Rectangle(0, 0, 75, 48);
            cameraOffset = new Vector2(0, 0);
            _playerState = PlayerState.Jump;
            _rayCastLength = 40f;
            speed = 1;
            _maxSpeed = 4;
            _jumpForce = -30;
            _gravity = 2;

            InitializePlayerSprites("playerTileMap");
           
        }

        public void Update(GameTime gameTime, Tile[,] tile)
        {
            _currentKb = Keyboard.GetState();
            _mouse = Mouse.GetState();
            cameraOffset.X = position.X - worldPos.X;
            cameraOffset.Y = position.Y - worldPos.Y;
            Move(tile);
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
        /// Movement logic for player 
        /// </summary>
        private void Move(Tile[,] tile)
        {

            if (_onGround)
            {
                _velocity.Y = 0;
            }
           _velocity.Y += _gravity;

            worldPos.X += (int)_velocity.X;

            worldPos.Y += (int)_velocity.Y;


            //StayOnGround(tile);

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
        public void StayOnGround(Tile[,] _level)
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
    }
}
