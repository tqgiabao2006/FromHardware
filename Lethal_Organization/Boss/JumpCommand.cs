using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lethal_Organization
{
    internal class JumpCommand : ICommand<Boss>
    {
        private Action _spawnSpike;
        public bool Finished { set; get; }

        private Action<Boss.State> _setAnim;

        private Func<Boss.State, int> _getMaxIndex;

        private Vector2 _jumpForce;

        private float _gravity;

        private Func<Boss.State, int, bool> _checkFinishAnimation;

        private Func<bool> _checkOnGround;

        private bool _hasJump;

        private bool _onGround;

        private Action<AudioManager.SFXID, float, float, float> _playSFX;

        internal JumpCommand(Action<AudioManager.SFXID, float, float, float>  playSFX, Action spawnSpike, Action<Boss.State> setAnim, Func<Boss.State, int> getMaxIndex, Vector2 jumpForce, float gravity, Func<Boss.State, int, bool> checkFinishAnimation, Func<bool> onGround)
        {
            _playSFX = playSFX;

            _spawnSpike = spawnSpike;

            _setAnim = setAnim;

            _getMaxIndex = getMaxIndex;

            _jumpForce = jumpForce;

            _checkFinishAnimation = checkFinishAnimation;

            _checkOnGround = onGround;

            _gravity = gravity;

            _hasJump = false;

            _onGround = true;

         
        }
        public void Execute(Boss boss)
        {
            _setAnim(Boss.State.Jump);
            boss.BossState = Boss.State.Jump;
        }


        public void Update(Boss boss, GameTime gameTime)
        {
            if(_hasJump && !_onGround)
            {
                boss.Velocity += new Vector2(0, _gravity);
            }

            if (_checkFinishAnimation(Boss.State.Jump, _getMaxIndex(Boss.State.Jump) - 1) && !_hasJump)
            {
                if(!boss.FaceRight)
                {
                    _jumpForce.X *= -1;
                }

                boss.LockDirection = true;
                boss.Velocity = _jumpForce;
                boss.BossState = Boss.State.MidAir;
                _setAnim(Boss.State.MidAir);
                _hasJump = true;
                _onGround = false;
            }

            if (_hasJump)
            {
                if (_checkOnGround() && !_onGround)
                {
                    _onGround = true;
                    boss.Velocity = Vector2.Zero;
                    _setAnim(Boss.State.Fall);
                    boss.BossState = Boss.State.Fall;
                }

                if (_checkFinishAnimation(Boss.State.Fall, _getMaxIndex(Boss.State.Fall) - 1))
                {
                    //Spawn spike
                    _playSFX(AudioManager.SFXID.IceCrack, 1, 0, 0);

                    boss.LockDirection = false;
                    _spawnSpike();
                    Finished = true;
                }
            }
           
        }
    }
}
