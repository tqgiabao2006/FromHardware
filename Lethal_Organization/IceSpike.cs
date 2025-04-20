using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    class IceSpike
    {
        private enum Spike
        {
            None
        }

        private bool _enabled;

        private int _damage;

        private int _hitBoxWidth;

        private int _hitBoxHeight;

        private Animator<Spike> _animator;

        private Rectangle _hitBox;

        private bool _waitAnim;

        public Rectangle HitBox
        {
            get { return _hitBox; }
        }

        private Player _player;

        public IceSpike(Texture2D texture, int groundWidth, Player player)
        {
            _player = player;
            _hitBoxHeight = 48;
            _hitBoxWidth = groundWidth;
            SetActive(false);
        }

        public void Spawn(Texture2D texture, Vector2 spawnPos)
        {
            if(_animator == null)
            {
                _animator = new Animator<Spike>(texture, Spike.None, 1920, 48, 0.1f);
            }

            _hitBox = new Rectangle((int)spawnPos.X, (int)spawnPos.Y, _hitBoxWidth, _hitBoxHeight);
            SetActive(true);
            _waitAnim = true;
            
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);
            if(_waitAnim)
            {
                if (_hitBox.Intersects(_player.WorldPos) && _animator.CheckAnimationFinish(Spike.None, _animator.GetMaxIndex(Spike.None) - 1))
                {
                    _player.GetHit(_damage);

                    SetActive(false);
                }
            }
        }

        public void Draw(SpriteBatch sb, Vector2 playerCameraOffset, bool isDebug)
        {
            Rectangle displayPos = new Rectangle(_hitBox.X + (int)playerCameraOffset.X, _hitBox.Y + (int)playerCameraOffset.Y, _hitBoxWidth, _hitBoxHeight);
            _animator.Draw(sb,displayPos, 0f, Color.White);
            
            if(isDebug)
            {
                CustomDebug.DrawWireRectangle(sb, _hitBox, 3, Color.Chocolate);
            }
        }

        public void SetActive(bool active)
        {
            _enabled = active;
        }



    }
}
