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
        private bool _enemyDirection = true;

        //temp values
        private Player _player;
        private int _playerXPos;
        private Rectangle _platform;
        private Vector2 _velocity;



        public Enemy(Texture2D sprite, Rectangle platform, Player player, GameManager gameManager)
        {
            _player = player;
            texture = sprite;
            displayPos = new Rectangle(platform.X, platform.Y - sourceImg.Height, 48, 48);
            speed = 5;
            _velocity = Vector2.Zero;
            _platform = platform;

            gameManager.StateChangedAction += OnStateChange;
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
            _playerXPos = _player.CameraPos.X;
            displayPos.X +=(int)_velocity.X;
            displayPos.Y += (int)_velocity.Y;
            switch(_state)
            {
                case EnemyState.Patrol:
                    Patrol();
                    //moves enemy based on which way it's facing
                    if (_enemyDirection)
                    {
                        _velocity.X = speed;
                    }
                    if (!_enemyDirection)
                    {
                        _velocity.X = -speed;
                    }
                    //turns enemy around at the edge of a platform
                    if (displayPos.X <= _platform.X)
                    {
                        _enemyDirection = true;
                    }
                    if (displayPos.X + displayPos.Width >= _platform.X + _platform.Width)
                    {
                        _enemyDirection = false;
                    }
                    
                    break;

                case EnemyState.Chase:
                    Chase();
                    //moves enemy based on which way it's facing
                    if (_enemyDirection)
                    {
                        _velocity.X = speed;
                    }
                    if (!_enemyDirection)
                    {
                        _velocity.X = -speed;
                    }
                    //turns enemy around at the edge of a platform
                    if (Math.Abs(displayPos.X - _playerXPos) <= 50 && _playerXPos >= displayPos.X)
                    {
                        _enemyDirection = true;
                    }
                    if (Math.Abs(displayPos.X - _playerXPos) <= 50 && _playerXPos < displayPos.X)
                    {
                        _enemyDirection = false;
                    }


                    break;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if(visible)
            {
                sb.Draw(
                 texture,
                 displayPos,
                Color.White);
            }
        }

        private void Patrol()
        {

        }

        private void Chase()
        {

        }
    }
}
