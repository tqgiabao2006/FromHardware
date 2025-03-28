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
        private KeyboardState _currentKb, _prevKb;
        private MouseState _mouse;
        public bool _onGround;
        public PlayerState _playerState;
        public float _rayCastLength;
        private Tile[,] _level;
        private Vector2 _playerVelocity = Vector2.Zero;
        private Vector2 _jumpVelocity = new Vector2(0, -15.0f);
        private Vector2 _gravity = new Vector2(0, 0.5f);

        public Rectangle Position => position;

        public Player(Texture2D sprite, Tile[,] tile)
        {
            texture = sprite;
            position = new Rectangle(0, 0, 75, 48);
            sourceImg = new Rectangle(0, 0, 75, 48);
            _level = tile;
            _playerState = PlayerState.Jump;
            _rayCastLength = 40f;
            speed = new Vector2(5, 0);
        }

        public override void Update(GameTime gameTime)
        {
            _currentKb = Keyboard.GetState();
            _mouse = Mouse.GetState();
            Move();
            _playerVelocity += _gravity;
            position.Y += (int)_playerVelocity.Y;
            _prevKb = _currentKb;
        }

        public override void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(texture, position, Color.White);
            if (isDebug) CustomDebug.DrawWireRectangle(sb, position, 0.5f, Color.Red);
        }

        private void Move()
        {
            _onGround = OnGround();
            switch (_playerState)
            {
                case PlayerState.Idle:
                    if ((_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left) ||
                         _currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right)) && _onGround)
                        _playerState = PlayerState.Run;
                    else if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) || _currentKb.IsKeyDown(Keys.Space)) && _onGround)
                    {
                        _playerVelocity = _jumpVelocity;
                        _playerState = PlayerState.Jump;
                    }
                    break;
                case PlayerState.Run:
                    if (_currentKb.IsKeyDown(Keys.A) || _currentKb.IsKeyDown(Keys.Left))
                        position.X -= (int)speed.X;
                    else if (_currentKb.IsKeyDown(Keys.D) || _currentKb.IsKeyDown(Keys.Right))
                        position.X += (int)speed.X;
                    if (_currentKb.GetPressedKeyCount() == 0) _playerState = PlayerState.Idle;
                    else if ((_currentKb.IsKeyDown(Keys.W) || _currentKb.IsKeyDown(Keys.Up) || _currentKb.IsKeyDown(Keys.Space)) && _onGround)
                    {
                        _playerVelocity = _jumpVelocity;
                        _playerState = PlayerState.Jump;
                    }
                    break;
                case PlayerState.Jump:
                    if (_onGround)
                    {
                        StayOnGround();
                        _playerState = PlayerState.Idle;
                    }
                    break;
            }
        }

        public bool OnGround()
        {
            foreach (var tile in _level)
            {
                if (tile == null) continue;
                if (OnHead(tile.PosRect)) return true;
            }
            return false;
        }

        public void StayOnGround()
        {
            foreach (var tile in _level)
            {
                if (tile == null) continue;
                if (Collides(tile.PosRect))
                {
                    Rectangle collidedObj = CollisionWith(tile.PosRect);
                    if (position.Y + position.Height >= collidedObj.Y)
                        position.Y = collidedObj.Y - position.Height;
                }
            }
        }
    }
}

