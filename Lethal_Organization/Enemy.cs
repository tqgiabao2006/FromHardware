using Microsoft.Xna.Framework;
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
        // false : left, true : right
        private bool _enemyDirection = false;
        private int _playerXPos;
        private Rectangle _tempPlatform;
        public override void Update(GameTime gameTime)
        {
            switch(_state)
            {
                case EnemyState.Patrol:
                    Patrol();
                    if (_enemyDirection)
                    {
                        position.X += (int)speed.X;
                    }
                    if (!_enemyDirection)
                    {
                        position.X -= (int)speed.X;
                    }
                    if (position.X <= _tempPlatform.X)
                    {
                        _enemyDirection = true;
                    }
                    if (position.X + position.Width >= _tempPlatform.X + _tempPlatform.Width)
                    {
                        _enemyDirection = false;
                    }
                    break;

                case EnemyState.Chase:
                    Chase();
                    break;
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
