using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Lethal_Organization
{
    internal class PunchCommand : ICommand<Boss>
    {
        public bool Finished { set; get; }

        private bool _faceRight;

        private bool _waitAnim;

        private float _speed;

        private float _punchRange;

        private int _damage;

        private int _damgeFrame;

        private int _endFrame;

        private Rectangle _punchHitbox;

        private Player _player;

        private Action<Boss.State> _setAnim;

        Func<Boss.State, int, bool> _checkFinishAnimation;

        private bool _canDamage;

        private float _sinceRunTime;

        private float _maxTimeToPunch; //Max time to find place to punch


        private Action<AudioManager.SFXID, float, float, float> _playSFX;


        internal PunchCommand(Action<AudioManager.SFXID, float, float, float> playSFX, float speed, int damage, int endFrame, Rectangle punchHitBox, Player player, Action<Boss.State> setAnim, Func<Boss.State, int, bool> checkFinishAnimation)
        {
            _playSFX = playSFX;

            _damgeFrame = 6;

            _endFrame = endFrame;

            _punchRange = punchHitBox.Width;

            _punchHitbox = punchHitBox;

            _speed = speed;

            _player = player;

            _checkFinishAnimation = checkFinishAnimation;

            _damage = damage;

            Finished = false;

            _waitAnim = false;

            _setAnim = setAnim;

            _canDamage = true;

            _sinceRunTime = 0;

            _maxTimeToPunch = 2;
        }

        public void Execute(Boss boss)
        {
            _setAnim(Boss.State.Idle);
        }

        public void Update(Boss boss, GameTime gameTime)
        {
            _punchHitbox.X = boss.WorldPos.Center.X;
            _punchHitbox.Y = boss.WorldPos.Y;
            if (_waitAnim)
            {
                if (_checkFinishAnimation(Boss.State.Punch, _damgeFrame))
                {
                    boss.LockDirection = true;

                    if (!_faceRight)
                    {
                        _punchHitbox.X = _punchHitbox.X - _punchHitbox.Width;
                    }

                    if (_punchHitbox.Intersects(_player.WorldPos) && _canDamage)
                    {
                        _player.GetHit(_damage);
                        _canDamage = false;
                    }

                }

                if (_checkFinishAnimation(Boss.State.Punch, _endFrame))
                {
                    boss.LockDirection = false;
                    Finished = true;
                }

                boss.Velocity = Vector2.Zero;
            }
            else
            {
                if (boss.WorldPos.Intersects(_player.HitBox) || _sinceRunTime > _maxTimeToPunch)
                {
                    _playSFX(AudioManager.SFXID.Swoosh, 1, 0, 0);
                    _setAnim(Boss.State.Punch);
                    _faceRight = _player.WorldPos.X > boss.HitBox.X;
                    _waitAnim = true;
                }
                else
                {
                    _setAnim(Boss.State.Walk);

                    _sinceRunTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    int dirMultipler = boss.FaceRight ? 1 : -1;

                    boss.Velocity = new Vector2(dirMultipler * _speed, 0);

                    boss.BossState = Boss.State.Walk;
                }
            }

        }
    }

}
