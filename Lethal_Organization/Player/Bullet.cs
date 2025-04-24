
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lethal_Organization;

internal class Bullet
{
    private enum State
    {
        None,
        Fly,
        Hit
    }

    private State _curState;
    
    private bool _enabled;

    private int _speed;
    
    private int _hitBoxRadius;

    private int _range;

    private int _damge;
    
    private Vector2 _direction;

    private Level _level;
    
    // Asset
    private Texture2D _spriteSheet;

    private Animator<State> _animator;

    private bool _waitForAnim;

    private bool _multipleAnimation;

    private float _rotation;
    
    //Positions
    private Rectangle _displayPos; //For display only
              
    private Rectangle _sourceImg; //For render from sprite sheet
            
    private Rectangle _worldPos; //Real position to do with logic

    private Vector2 _spawnPos;

    //Targets (either Player (for Boss), or Enemy  + Boss for player)
    private List<Enemy> _enemyList;

    private Player _player;

    private Boss _boss;

    //Deal damge

    private bool _canDealDamge; //Avoid get hit mutliple frame

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
    }
    
    public Vector2 WorldPos
    {
        get
        {
            return new Vector2(_worldPos.X, _worldPos.Y); 
        }
        private set
        {
            _worldPos.X = (int)value.X;
            _worldPos.Y = (int)value.Y;
        }
    }

    public float Rotation
    {
        get
        {
         return _rotation;
        }
    }

    public Bullet(Texture2D spriteSheet, Level level)
    {
        this._level = level;
        this.SetActive(false);
        this._spriteSheet = spriteSheet;
    }
    
    
    /// <summary>
    /// Pick to spawn bullet with multiple state
    /// </summary>
    /// <param name="spriteMapFile"></param>
    /// <param name="worldPos"></param>
    /// <param name="direction"></param>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <param name="frameWidth"></param>
    /// <param name="frameHeight"></param>
    /// <param name="framePerSecond"></param>
    /// <param name="hitBoxRadius"></param>
    /// <param name="range"></param>
    public void Spawn(string spriteMapFile, Player player, float angle,Vector2 worldPos, Vector2 direction, int damage, int speed, int frameWidth, int frameHeight, int scale, float framePerSecond = 0.05f, int hitBoxRadius = 10, int range = 1000)
    {
        _curState = State.Fly;
        
        _canDealDamge = true;

        if (_animator == null)
        {
            _animator = new Animator<State>(_spriteSheet, State.None, spriteMapFile, 0.1f);
        }

        if(_player == null)
        {
            _player = player;
        }

        _waitForAnim = false;
        
        _spawnPos = worldPos;
       
        this._hitBoxRadius =  hitBoxRadius;
        this._range = range;
        this._damge = damage;
        this._speed = speed;

        _multipleAnimation = true;

        _rotation = angle;

        this._sourceImg = new Rectangle(0, 0, frameWidth, frameHeight);
        this._worldPos = new Rectangle(500,500, frameWidth * scale, frameHeight * scale);

        this._worldPos.X = (int)worldPos.X - (scale - 1) * frameWidth; //Adjust world position to fi with scale
        this._worldPos.Y = (int)worldPos.Y - (scale - 1) * frameHeight;

        this._enabled = true;
        this._direction = direction;


        _animator.SetState(_curState);

        _direction.Normalize();
    }
    
    

    /// <summary>
    /// Spawn bullet with only 1 state - animation loop
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="direction"></param>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <param name="frameWidth"></param>
    /// <param name="frameHeight"></param>
    /// <param name="framePerSecond"></param>
    /// <param name="hitBoxRadius"></param>
    /// <param name="range"></param>
    public void Spawn(Vector2 worldPos, List<Enemy> enemyList, Boss boss,Vector2 direction,int damage, int speed, int frameWidth, int frameHeight, int scale,float framePerSecond = 0.05f, int hitBoxRadius = 10, int range = 1000)
    {
        _curState = State.Fly;

        _canDealDamge = true;
        
        if (_animator == null)
        {
            _animator = new Animator<State>(_spriteSheet, State.Fly, frameWidth, frameHeight, framePerSecond, true);
        }

        if (_enemyList == null)
        {
            _enemyList = enemyList;
        }

        if(_boss == null)
        {
            _boss = boss;
        }

        _waitForAnim = false;

        _multipleAnimation = false;

        _spawnPos = worldPos;
       
        this._hitBoxRadius =  hitBoxRadius;
        this._range = range;
        this._damge = damage;
        this._speed = speed;

        this._sourceImg = new Rectangle(0, 0, frameWidth, frameHeight);
        this._worldPos = new Rectangle(0, 0, frameWidth * scale, frameHeight*scale);

        this._worldPos.X= (int)worldPos.X - (scale - 1) * frameWidth; //Adjust world position to fi with scale
        this._worldPos.Y = (int)worldPos.Y - (scale - 1 ) * frameHeight;
       
        this._enabled = true;
        this._direction = direction;
        _direction.Normalize();

    }
    public void SetActive(bool active)
    {
        _enabled = active;
    }

    /// <summary>
    /// Deal damage, used by boss, enemy 
    /// </summary>
    /// <param name="player">target of bullet</param>
    private void DealDamge(Player player)
    {
        if(player == null)
        {
            return;
        }

        if (this.CollideWith(player.WorldPos) && player.Enabled && _canDealDamge)
        {
            player.GetHit(_damge);
            _canDealDamge = false;
            Destroy();
            return;
        }

    }

    /// <summary>
    /// Used by player, deal damage
    /// </summary>
    /// <param name="enemyList">target</param>
    /// <param name="boss">target</param>
    private void DealDamge(List<Enemy> enemyList, Boss boss)
    {
        if(enemyList == null || boss ==null)
        {
            return;
        }

        if(this.CollideWith(boss.HitBox) && boss.Enabled && _canDealDamge)
        {
            boss.GetHit(_damge);
            _canDealDamge = false;
            Destroy();
        }

        foreach (Enemy enemy in enemyList)
        {
            if(this.CollideWith( enemy.WorldPos) && enemy.Enabled && _canDealDamge)
            {
                enemy.GetHit(_damge);
                Destroy();
                _canDealDamge = false;
                return;
            }
        }
    }

    /// <summary>
    /// Check collsion, disabled bullet when they hit
    /// </summary>
    private void CheckCollision()
    {
        if (_level == null)
        {
            return;
        }

        for (int i = 0; i < _level.SizeX; i++)
        {
            for (int j = 0; j < _level.SizeY; j++)
            {
                if (_level[i,j] != null && _level[i,j].Type != Level.TileType.Decoration)
                {
                    Rectangle tilePos = _level[i, j].WorldPos;
                    if (tilePos.Intersects(_worldPos))
                    {
                        _curState = State.Hit;
                        _animator.SetState(_curState);
                        Destroy();
                        return;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Update the bullet fly
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        if (!_enabled)
        {
            return;
        }

        if (_waitForAnim)
        {
            if (_animator.CheckAnimationFinish(State.Hit, _animator.GetMaxIndex(State.Hit) - 1))
            {
                SetActive(false);
            }
        }

        _animator.Update(gameTime);

        CheckCollision();

        if(_enemyList != null && _boss != null)
        {
            DealDamge(_enemyList, _boss);
        }

        if(_player != null)
        {
            DealDamge(_player);
        }

        if (!_waitForAnim)
        {
            this._worldPos.X += (int)(_direction.X * _speed);

            this._worldPos.Y += (int)(_direction.Y * _speed);
        }
       

        if (Vector2.Distance(_spawnPos, new Vector2(_worldPos.X, _worldPos.Y)) > _range)
        {
            SetActive(false);
            return;
        }
    }

    private void Destroy()
    {
        //If contain "impact" animation => wait before set active
        if (_multipleAnimation)
        {
            _animator.SetState(State.Hit); 
            _waitForAnim = true;
        }
        else //If not, set active to false immidately when collide
        {
            SetActive(false);
        }
    }
       
    public void Draw(SpriteBatch sb, bool isDebug, Vector2 cameraOffset)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)cameraOffset.X,(int)_worldPos.Y + (int)cameraOffset.Y, _worldPos.Width, _worldPos.Height); //Lock y axis

        SpriteEffects effect = _direction.X  > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally; 
        if (_enabled)
        {
            _animator.Draw(sb, _displayPos, effect, Color.White);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(_worldPos.Center.X, _worldPos.Center.Y),  _hitBoxRadius, 3, Color.Red);
            CustomDebug.DrawWireCircle(sb, new Vector2(_displayPos.Center.X, _displayPos.Center.Y), _hitBoxRadius, 3, Color.Yellow);
        }
    }

    public void Draw(SpriteBatch sb, bool isDebug, Vector2 cameraOffset, float angle, int scale = 1)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)cameraOffset.X - (scale - 1) * _worldPos.Width, (int)_worldPos.Y - (scale - 1) *_worldPos.Height  + (int)cameraOffset.Y,
            _worldPos.Width * scale, _worldPos.Height * scale); 

        if (_enabled)
        {
            _animator.Draw(sb, _displayPos, angle, Color.White  );
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(_worldPos.X, _worldPos.Y), _hitBoxRadius, 3, Color.Red);
            CustomDebug.DrawWireCircle(sb, new Vector2(_displayPos.X, _displayPos.Y), _hitBoxRadius, 3, Color.Yellow);
        }
    }

    /// <summary>
    /// Collision between circle and rectangle
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool CollideWith(Rectangle target)
    {
        float closestX = MathHelper.Clamp(_worldPos.Center.X, target.X, target.X + target.Width);
        float closestY = MathHelper.Clamp(_worldPos.Center.Y, target.Y, target.Y + target.Height);

        float distance = Vector2.Distance(new Vector2(_worldPos.Center.X, _worldPos.Center.Y), new Vector2(closestX, closestY));

        return distance < _hitBoxRadius;
    }
}