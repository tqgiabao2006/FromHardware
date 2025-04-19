using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Lethal_Organization
{
    internal class SpikeCommand : ICommand<Boss>
    {
        public bool Finished { set; get; }

        private Action<Vector2, Vector2, float> _spawnBullet;

        private Action<Boss.State> _setAnim;

        Func<Boss.State, int, bool> _checkFinishAnimation;

        private int _numbSpikes;

        private float _radius;

        private int _endFrame;

        public SpikeCommand(Action<Vector2, Vector2, float> spawnBullet, Action<Boss.State> setAnim, Func<Boss.State, int, bool> checkFinishAnimation, int numbSpikes, float radius, int endFrame)
        {
            _spawnBullet = spawnBullet;

            _radius = radius;

            _checkFinishAnimation = checkFinishAnimation;

            _numbSpikes = numbSpikes;

            _setAnim = setAnim;

            _endFrame = endFrame;
        }


        public void Execute(Boss gameObject)
        {
            _setAnim(Boss.State.Spike);
        }

        public void Update(Boss boss, GameTime gameTime)
        {
            if (Finished)
            {
                boss.BossState = Boss.State.Idle;
                _setAnim(Boss.State.Idle);
                return;
            }

            if (_checkFinishAnimation(Boss.State.Spike, _endFrame))
            {
                float step = 360f / _numbSpikes;

                for (float i = 0; i < 360; i += step)
                {
                    float radians = MathHelper.ToRadians(i);
                    Vector2 direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians));

                    Vector2 spawnPos = new Vector2(boss.HitBox.Center.X, boss.HitBox.Center.Y);
                    _spawnBullet(direction, spawnPos, radians);
                }

                Finished = true;
            }
        }
    }
}
