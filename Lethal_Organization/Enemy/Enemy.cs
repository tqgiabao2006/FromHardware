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

        private Vector2 _rightPlatform;

        private Vector2 _leftPlatform;

        private Vector2 _velocity;

        private float _chasingRadius;

        private bool _hasChased; //This to avoid enemy turn back and froth immidately
                                 //when player move around edge of checking radius

        //Get hit
        private bool _getHit; //Track get hit frame to show health bar, change color 1 framce

        private float _timeSinceHit;

        private float _showHPTime;

        private bool _changeColorGetHit;

        private float _sinceChangeColor;

        private float _changeColorTime;

        //Render
        private HealthBar _healthBar;

        private Animator<EnemyState> _animator;

        //SFX
        private AudioManager _audioManager;
        public Rectangle WorldPos
        {
            get 
            { 
                return worldPos; 
            }
        }

        public Enemy(Texture2D sprite, Texture2D UISprite, Rectangle healthBarSourceImg, Vector2 rightPlatform, Vector2 leftPlatform, Player player, GameManager gameManager, AudioManager audioManager,int frameWidth, int frameHeight, int speed, int scale = 2)
        {
            _player = player;

            texture = sprite;

            _audioManager = audioManager;

            _rightPlatform = rightPlatform;

            _leftPlatform = leftPlatform;
            
            this.speed = speed;

            curHP = 100;

            maxHp = 100;

            _showHPTime = 3;
            
            _velocity = Vector2.Zero;

            _chasingRadius = 100;

            _faceRight = false;

            gameManager.OnStateChange += OnStateChange;

            //UI_Render
            worldPos = new Rectangle((int)rightPlatform.X, (int)rightPlatform.Y - scale * frameHeight, frameWidth * scale, frameHeight * scale);

            _healthBar = new HealthBar(this, UISprite, healthBarSourceImg, new Rectangle(this.worldPos.X, this.worldPos.Y - 20, healthBarSourceImg.Width * 3, healthBarSourceImg.Height * 3));

            displayPos = new Rectangle(0, 0, 48, 48);

            _animator = new Animator<EnemyState>(sprite, EnemyState.Chase, frameWidth, frameHeight, 0.05f, true);

            _changeColorTime = 0.2f;

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
                    if(enabled)
                    {
                        visible = true;
                    }
                    paused = false;
                    isDebug = false;

                    break;

                case GameManager.GameState.GameOver:
                    visible = false;
                    paused = false;
                    isDebug = false;

                    break;
                case GameManager.GameState.Die:
                case GameManager.GameState.Pause:
                    paused = true;
                    break;
                case GameManager.GameState.Debug:
                    paused = false;
                    isDebug = true;
                    break;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if(paused || !visible)
            {
                return;
            }

            worldPos.X +=(int)_velocity.X;
            worldPos.Y += (int)_velocity.Y;


            _animator.Update(gameTime);

            DisplayHP(gameTime);

            GetHitEffect(gameTime);

            float dst = Vector2.DistanceSquared(new Vector2(_player.WorldPos.Center.X, _player.WorldPos.Center.Y), new Vector2( worldPos.Center.X, worldPos.Center.Y));
            float xDistance = _player.WorldPos.Center.X - worldPos.Center.X;
            
            _velocity.X = _faceRight ? speed : -speed;

            switch (_state)
            {
                case EnemyState.Patrol:

                    if(!_faceRight && this.worldPos.X < _leftPlatform.X)
                    {
                        _faceRight = true;
                        if (_hasChased && this.worldPos.X < (_rightPlatform.X - _leftPlatform.X) * 0.7f)
                        {
                            _hasChased = false;
                        }

                    }
                    else if(_faceRight && this.worldPos.X  + this.worldPos.Width > _rightPlatform.X)
                    {
                        //Allow chase after move over the 70% of the path
                        if(_hasChased && this.worldPos.X > (_rightPlatform.X - _leftPlatform.X) * 0.7f)
                        {
                            _hasChased = false;
                        }
                        _faceRight = false;
                    }

            
                    if(Math.Abs(dst) < _chasingRadius * _chasingRadius && !_hasChased)
                    {
                        _state = EnemyState.Chase;
                    }


                    break;

                case EnemyState.Chase:

                    //Lock direction, avoid enemy turn back and forth too fast
                    if(xDistance < 0 && !_hasChased)
                    {
                        _faceRight = false;
                        _hasChased = true;
                    }
                    else if(xDistance > 0 )
                    {
                        _faceRight = true;
                    }

                    if (Math.Abs(dst) > _chasingRadius * _chasingRadius || this.worldPos.X < _leftPlatform.X || this.worldPos.X > _rightPlatform.X)
                    {
                        _state = EnemyState.Patrol;
                    }
                    break;
            }

        }

        public void Draw(SpriteBatch sb, Player player)
        {

            displayPos = new Rectangle(worldPos.X + (int)player.CameraOffset.X, worldPos.Y + (int)player.CameraOffset.Y, worldPos.Width, worldPos.Height);

            if (visible)
            {
                SpriteEffects effect = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Color color = _changeColorGetHit ? Color.Red : Color.White;

                _animator.Draw(sb, displayPos, effect, color);

                _healthBar.UpdatePos(new Vector2(displayPos.X, displayPos.Y - 20));

                if (_getHit)
                {
                    _healthBar.Draw(sb);
                }

            }

            if (isDebug && enabled)
            {
                Color color = _state == EnemyState.Patrol ? Color.Yellow : Color.Red;
                CustomDebug.DrawWireCircle(sb, new Vector2(displayPos.Center.X, displayPos.Center.Y), _chasingRadius, 3, color);

                CustomDebug.DrawWireCircle(sb, new Vector2(worldPos.Center.X, worldPos.Center.Y), _chasingRadius, 3, color);


                CustomDebug.DrawWireCircle(sb, _leftPlatform, 20, 3, Color.Purple);

                CustomDebug.DrawWireCircle(sb, _rightPlatform, 20,3, Color.Purple);

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

        private void DisplayHP(GameTime gameTime)
        {

            if(_getHit)
            {
                _timeSinceHit += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _healthBar.Visible = true;

                if(_timeSinceHit >= _showHPTime || curHP <= 0)
                {
                    _healthBar.Visible = false;
                    _getHit = false;
                }
            }
        }

        public override void GetHit(int damage)
        {
            base.GetHit(damage);
            _getHit = true;
            _timeSinceHit = 0;

            _sinceChangeColor = 0;
            _changeColorGetHit = true;

            _audioManager.PlaySFX(AudioManager.SFXID.GetHit);
        }

        private void GetHitEffect(GameTime gameTime)
        {
            if(_changeColorGetHit)
            {
                _sinceChangeColor +=(float) gameTime.ElapsedGameTime.TotalSeconds;

                if(_sinceChangeColor >= _changeColorTime || curHP <=0)
                {
                    _changeColorGetHit = false;
                    _sinceChangeColor = 0;
                }
            }
        }

    }
}
