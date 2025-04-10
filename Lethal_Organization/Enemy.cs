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
    internal class Enemy : GameObject
    {
        private EnemyState _state = EnemyState.Patrol;
        // false : left  ,  true : right
        private bool _enemyDirection = true;

        //temp values
        private int _playerXPos;
        private Rectangle _tempPlatform = new Rectangle(50, 200, 200, 10);
        private Vector2 _velocity;


        public Enemy(Texture2D sprite, Player player)
        {
            texture = sprite;
            displayPos = new Rectangle(10, 10, 48, 48);
            speed = 5;
            _velocity = Vector2.Zero;
        }


        public override void Update(GameTime gameTime)
        {
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
                    if (displayPos.X <= _tempPlatform.X)
                    {
                        _enemyDirection = true;
                    }
                    if (displayPos.X + displayPos.Width >= _tempPlatform.X + _tempPlatform.Width)
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

        public override void Draw(SpriteBatch sb, bool isDebug)
        {
            sb.Draw(
                texture,
                displayPos,
                Color.White);
        }

        private void Patrol()
        {

        }

        private void Chase()
        {

        }
    }
}
