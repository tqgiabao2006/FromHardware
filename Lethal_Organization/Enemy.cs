using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{

    public enum EnemyState
    {
        Patrol,
        Chase
    }
    internal class Enemy : GameObject, IStateChange
    {
        private EnemyState _state = EnemyState.Patrol;
        // false : left  ,  true : right
        private bool _faceRight = true;

        private Player _player;

        private int _playerXPos;

        private Rectangle _rightPlatform;

        private Rectangle _leftPlatform;

        private Vector2 _velocity;

        private bool getHit; //Track get hit frame to show health bar, change color 1 framce

        private float _timeCounter;

        private float _showHPTime;

        public delegate void ShowHP();

        public event ShowHP showHP;

        public delegate void HideHP();

        public event HideHP hideHP;

        public Rectangle WorldPos
        {
            get 
            { 
                return worldPos; 
            }
        }

        public Enemy(Texture2D sprite, Rectangle rightPlatform, Rectangle leftPlatform, Player player, GameManager gameManager)
        {
            _player = player;

            texture = sprite;

            worldPos = new Rectangle(rightPlatform.X, rightPlatform.Y - 48, 48, 48);

            displayPos = new Rectangle(0, 0, 48, 48);

            speed = 2;

            _showHPTime = 5;

            _showHPTime = speed;
            
            _velocity = Vector2.Zero;

            _rightPlatform = rightPlatform;

            _leftPlatform = leftPlatform;

            gameManager.OnStateChange += OnStateChange;
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
            DisplayHP();

            _playerXPos = _player.WorldPos.X;
            worldPos.X +=(int)_velocity.X;
            worldPos.Y += (int)_velocity.Y;
            switch(_state)
            {
                case EnemyState.Patrol:
                    //moves enemy based on which way it's facing
                    if (_faceRight)
                    {
                        _velocity.X = speed;
                    }
                    if (!_faceRight)
                    {
                        _velocity.X = -speed;
                    }
                    //turns enemy around at the edge of a platform
                    if (worldPos.X <= _leftPlatform.X)
                    {
                        _faceRight = true;
                    }
                    if (worldPos.X + worldPos.Width >= _rightPlatform.X + _rightPlatform.Width)
                    {
                        _faceRight = false;
                    }
                    
                    break;

                case EnemyState.Chase:
                    //moves enemy based on which way it's facing
                    if (_faceRight)
                    {
                        _velocity.X = speed;
                    }
                    if (!_faceRight)
                    {
                        _velocity.X = -speed;
                    }
                    //turns enemy around at the edge of a platform
                    if (Math.Abs(worldPos.X - _playerXPos) <= 50 && _playerXPos >= worldPos.X)
                    {
                        _faceRight = true;
                    }
                    if (Math.Abs(worldPos.X - _playerXPos) <= 50 && _playerXPos < worldPos.X)
                    {
                        _faceRight = false;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch sb, Player player)
        {

            displayPos = new Rectangle(worldPos.X + (int)player.CameraOffset.X, worldPos.Y + (int)player.CameraOffset.Y, worldPos.Width, worldPos.Height);

            if (visible)
            {
                sb.Draw(
                 texture,
                 displayPos,
                Color.White);
            }

            if (isDebug)
            {
                if (worldPos.Intersects(player.WorldPos))
                {
                    CustomDebug.DrawWireRectangle(sb, worldPos, 3f, Color.Red);
                }
                else
                {
                    CustomDebug.DrawWireRectangle(sb, worldPos, 3f, Color.Aqua);
                }
            }
        }

        private void DisplayHP()
        {
            if(getHit && _timeCounter >0)
            {
                showHP();
                _timeCounter--;
                showHP?.Invoke();

            }else if(_timeCounter < 0 ||curHP <=0)
            {
                getHit = false;
                hideHP?.Invoke();
            }
        }

        public override void GetHit(int damage)
        {
            base.GetHit(damage);
            getHit = true;
            _timeCounter = _showHPTime;
        }

    }
}
