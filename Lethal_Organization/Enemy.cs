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

        private float _chasingRadius;

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



       

        public Rectangle WorldPos
        {
            get 
            { 
                return worldPos; 
            }
        }

        public Enemy(Texture2D sprite, Texture2D UISprite, Rectangle healthBarSourceImg,Rectangle rightPlatform, Rectangle leftPlatform, Player player, GameManager gameManager, int scale = 2)
        {
            _player = player;

            texture = sprite;

            worldPos = new Rectangle(rightPlatform.X, rightPlatform.Y * scale, 57* scale, 42*scale);

            _healthBar = new HealthBar(this, UISprite, healthBarSourceImg, new Rectangle(this.worldPos.X, this.worldPos.Y - 20, healthBarSourceImg.Width * 3, healthBarSourceImg.Height *3));

            displayPos = new Rectangle(0, 0, 48, 48);

            _animator = new Animator<EnemyState>(sprite, EnemyState.Chase, 57, 42, 0.05f);

            speed = 2;

            curHP = 100;

            maxHp = 100;

            _showHPTime = 5;
            
            _velocity = Vector2.Zero;

            _rightPlatform = rightPlatform;

            _leftPlatform = leftPlatform;

            _changeColorTime = 0.2f;

            _chasingRadius = 200;

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

            _playerXPos = _player.WorldPos.X;
            worldPos.X +=(int)_velocity.X;
            worldPos.Y += (int)_velocity.Y;


            _animator.Update(gameTime);

            DisplayHP(gameTime);

            GetHitEffect(gameTime);

            float distance = _player.WorldPos.Center.X - worldPos.Center.X;

            switch (_state)
            {
                case EnemyState.Patrol:
                    _velocity.X = _faceRight ? speed : -speed;

                    if (worldPos.X <= _leftPlatform.X)
                    {
                        _faceRight = true;
                    }
                    else if (worldPos.X + worldPos.Width >= _rightPlatform.X + _rightPlatform.Width)
                    {
                        _faceRight = false;
                    }

                    if(distance < _chasingRadius)
                    {
                        _state = EnemyState.Chase;
                    }
                    break;

                case EnemyState.Chase:

                    _faceRight = distance < 0;

                    _velocity.X = _faceRight ? speed : -speed;
                    
                    if(distance > _chasingRadius || this.worldPos.X > _rightPlatform.X || this.worldPos.X < _rightPlatform.X)
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
                CustomDebug.DrawWireCircle(sb, new Vector2(displayPos.Center.X, displayPos.Center.Y), _chasingRadius, 3, Color.Yellow);

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
