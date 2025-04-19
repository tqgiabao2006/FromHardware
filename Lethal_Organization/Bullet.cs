
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lethal_Organization;

public class Bullet
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
    public void Spawn(string spriteMapFile, float angle,Vector2 worldPos, Vector2 direction, int damage, int speed, int frameWidth, int frameHeight, float framePerSecond = 0.05f, int hitBoxRadius = 10, int range = 1000)
    {
        _curState = State.Fly;
        if (_animator == null)
        {
            _animator = new Animator<State>(_spriteSheet, State.None, spriteMapFile, 0.2f);
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
        this._worldPos = new Rectangle(0, 0, frameWidth, frameHeight);

        this._worldPos.X= (int)worldPos.X;
        this._worldPos.Y = (int)worldPos.Y;
       
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
    public void Spawn(Vector2 worldPos, Vector2 direction, int damage, int speed, int frameWidth, int frameHeight, float framePerSecond = 0.05f, int hitBoxRadius = 10, int range = 1000)
    {
        _curState = State.Fly;
        
        if (_animator == null)
        {
            _animator = new Animator<State>(_spriteSheet, State.Fly, frameWidth, frameHeight, framePerSecond);
        }

        _waitForAnim = false;

        _multipleAnimation = false;

        _spawnPos = worldPos;
       
        this._hitBoxRadius =  hitBoxRadius;
        this._range = range;
        this._damge = damage;
        this._speed = speed;

        this._sourceImg = new Rectangle(0, 0, frameWidth, frameHeight);
        this._worldPos = new Rectangle(0, 0, frameWidth, frameHeight);

        this._worldPos.X= (int)worldPos.X;
        this._worldPos.Y = (int)worldPos.Y;
       
        this._enabled = true;
        this._direction = direction;
        _direction.Normalize();

    }
    public void SetActive(bool active)
    {
        _enabled = active;
    }

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
                if (_level[i,j] != null)
                {
                    Rectangle tilePos = _level[i, j].WorldPos;
                    if (tilePos.Intersects(_worldPos))
                    {
                        _curState = State.Hit;
                        _animator.SetState(_curState);

                        //If contain "impact" animation => wait before set active
                        if (_multipleAnimation)
                        {
                            _waitForAnim = true;
                        }
                        else //If not, set active to false immidately when collide
                        { 
                            SetActive(false);
                        }
                        return;
                    }
                }
            }
        }
    }
    
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

        if(!_waitForAnim)
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
      
    public void Draw(SpriteBatch sb, bool isDebug, Vector2 cameraOffset, SpriteEffects effects)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)cameraOffset.X,(int)_worldPos.Y + (int)cameraOffset.Y, _worldPos.Width, _worldPos.Height); //Lock y axis

        if (_enabled)
        {
            _animator.Draw(sb, _displayPos, effects);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(_worldPos.X, _worldPos.Y),  _hitBoxRadius, 3, Color.Red);
            CustomDebug.DrawWireCircle(sb, new Vector2(_displayPos.X, _displayPos.Y), _hitBoxRadius, 3, Color.Yellow);
        }
    }

    public void Draw(SpriteBatch sb, bool isDebug, Vector2 cameraOffset, float angle)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)cameraOffset.X, (int)_worldPos.Y + (int)cameraOffset.Y, _worldPos.Width, _worldPos.Height); //Lock y axis

        if (_enabled)
        {
            _animator.Draw(sb, _displayPos, angle);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(_worldPos.X, _worldPos.Y), _hitBoxRadius, 3, Color.Red);
            CustomDebug.DrawWireCircle(sb, new Vector2(_displayPos.X, _displayPos.Y), _hitBoxRadius, 3, Color.Yellow);
        }
    }
}