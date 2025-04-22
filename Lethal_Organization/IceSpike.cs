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

        private Animator<Spike> _animator;

        private Rectangle _hitBox;

        private bool _waitAnim;

        private Player _player;

        private Texture2D _texture;

        private bool _canDealDamage;

        private float _sinceAnimEnd;

        private float _animLast;

        public IceSpike(Texture2D texture, Rectangle hitBox, Player player)
        {
            _texture = texture;
            _player = player;
            _hitBox = hitBox;
            _damage = 10;
            _sinceAnimEnd = 0;
            _animLast = 0.4f;
            SetActive(false);
        }

        public void Spawn()
        {
            if(_animator == null)
            {
                _animator = new Animator<Spike>(_texture, Spike.None, 1920, 48, 0.1f, false);
            }
            SetActive(true);
            _sinceAnimEnd = 0;
            _waitAnim = false;
            _canDealDamage = true;
       
        }

        public void Update(GameTime gameTime)
        {
            if(_animator == null || !_enabled)
            {
                return;
            }

            _animator.Update(gameTime);
           

            if (_animator.CheckAnimationFinish(Spike.None, _animator.GetMaxIndex(Spike.None) - 1))
            {
                if (_hitBox.Intersects(_player.WorldPos) && _canDealDamage)
                {
                    _player.GetHit(_damage);
                    _canDealDamage = false;
                }

                _waitAnim = true;
            }

            if(_waitAnim)
            {
                _sinceAnimEnd += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_sinceAnimEnd > _animLast)
                {
                    _sinceAnimEnd = 0;
                    this.SetActive(false);
                }
            }
        }

        public void Draw(SpriteBatch sb, Vector2 playerCameraOffset, bool isDebug)
        {
            Rectangle displayPos = new Rectangle(_hitBox.X + (int)playerCameraOffset.X, _hitBox.Y + (int)playerCameraOffset.Y - _hitBox.Height, _hitBox.Width, _hitBox.Height * 2);

            if (_enabled)
            {
                _animator.Draw(sb, displayPos, SpriteEffects.None, Color.White);
            }
           
            
            if(isDebug)
            {
                CustomDebug.DrawWireRectangle(sb, displayPos, 3, Color.Red);
            }
        }

        public void SetActive(bool active)
        {
            _enabled = active;
        }



    }
}
