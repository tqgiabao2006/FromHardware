using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

internal class Boss: GameObject
{
    public enum State
    {
        Revive,
        Idle,
        Walk,
        TakeHit,
        Punch,
        Spike,
        MidAir,
        Jump,
        Fall,
        Die
    }
    private bool _isFree;

    private State _currentState;

    private bool _dying;

    private Player _player;
    
    private Level _level;
    
    private Random _random;
    
    private ObjectPooling _objectPooling;
    
    private Animator<State> _animator;

    private List<Skill<State>> _skillSet;

    private Queue<ICommand<Boss>> _commandsQueue;
    
    //Asset
    private Texture2D _bulletTexture;

    private Texture2D _iceSpikeTexture;

    //Delegate
    private Action<GameManager.GameState> _changeState;

    private Action<Vector2, Vector2, float> _spawnBullet;

    private Action<State> _setAnim;

    private Func<bool> _checkOnGround;

    private Func<State, int,bool> _checkAnimFinish;

    private Func<State, int> _getMaxIndex;

    public event Action OnBossFightEvent;

    //Skill
    private IceSpike _iceSpike;

    private Action _spawnSpike;

    private bool _triggered;

    private Rectangle _punchHitBox;

    private Rectangle _hitBox;

    private Rectangle _groundBox;

    private int _phase2Hp;

    private float _generateCommandTime;

    private float _timeCounter;

    private float _processCommandTimeGap;

    private float _sinceLastCommand;

    //Movement
    private Vector2 _velocity;
    
    private Vector2 _jumpForce;

    private float _speed;

    private float _gravity;

    private bool _faceRight;

    //Shoot
    private int _spikeNumb;

    private float _radius;

    private int _bulletDamge;

    private int _bulletRange;

    private int _bulletHitBox;

    private int _bulletSpeed;

    //Command
    private ICommand<Boss> _curCommand;
    
    private Queue<ICommand<Boss>> _commandQueue;

    //Draw
    public bool LockDirection;

    private SpriteEffects _spriteEffects;

    //Get hit effect
    private bool _changeColorGetHit;

    private float _sinceChangeColor;

    private float _changeColorTime;

    private bool _hasGetHitFrame;

    //SFX
    private AudioManager _audioManager;

    private Action<AudioManager.SFXID,float, float, float> _playSFX;

    //Debug

    public Rectangle GroundBox
    {
        get
        {
            return _groundBox;
        }
    }
    public bool OnCommand
    {
        get { return _curCommand != null && !_curCommand.Finished; }
    }

    public bool Free
    {
        get
        {
            return _isFree;
        }
    }

    public int CurHP
    {
        get
        {
            return curHP;
        }
    }

 
    public Rectangle HitBox
    {
        get 
        {
            return _hitBox; 
        }
    }
    public bool FaceRight
    {
        get
        {
            return _faceRight;
        }
    }
    
    public Vector2 Velocity
    {
        get
        {
            return _velocity;   
        }
        set
        {
            _velocity = value;
        }
    }

    public Rectangle WorldPos
    {
        get
        {
            return worldPos;
        }
    }


    public State BossState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
        }
    }

    public Boss(Texture2D spriteSheet, Texture2D bulletTexture, Texture2D iceSpike,
        string textureMapFile,
        Player player, Level level,
        GameManager manager, AudioManager audioManager, Random random, ObjectPooling objectPooling)
    {
        //Class
        this._player = player;
        
        this._level = level;    
        
        this._currentState = State.Die;
        
        texture = spriteSheet;

        _audioManager = audioManager;

        _playSFX = _audioManager.PlaySFX;
        
        _bulletTexture = bulletTexture;
        
        //Render
        _iceSpikeTexture = iceSpike;

        _animator = new Animator<State>(spriteSheet, _currentState, textureMapFile, 0.1f);

        _skillSet = new List<Skill<State>>()
        {

            new Skill<State>
            {
                State = State.Spike,
                Rate = 30,
                Damage = 20
            },
            new Skill<State>
            {
                State = State.Punch,
                Rate = 30,
                Damage = 10
            },
            new Skill<State>
            {
                State = State.Jump,
                Rate = 30,
                Damage = 20,
            }
        };

        //Gethit effect
        _sinceChangeColor = 0;
        _changeColorGetHit = false;
        _changeColorTime = 0.1f;
      
        
        _commandQueue = new Queue<ICommand<Boss>>();
        
        this._random = random;
        
        _objectPooling = objectPooling;

        //Movement
        curHP = 200;

        maxHp = 200;
        
        _generateCommandTime = 2;

        _timeCounter = 3;

        _processCommandTimeGap = 2;

        _sinceLastCommand = 0;
        
        _velocity = new Vector2(0, 0);

        _speed = 3;

        _jumpForce = new Vector2(_speed * 3, -10);

        _gravity = 0.2f;

        //Shoot
        _spikeNumb = 30;

        _radius = 300;

        _bulletHitBox = 30;

        _bulletRange = 1500;

        _bulletSpeed = 8;

        _bulletDamge = 10;

        //Position_HitBox

        _groundBox = new Rectangle(4 * 48, 49 * 48, 32 * 48, 14 * 48); // Grid Pos (4,47), Width 32 Node, Height 14 Node

        worldPos = new Rectangle(_groundBox.Center.X,_groundBox.Y + _groundBox.Height - 96 * 3, 192 * 3, 96 *3);           //3 is scale

        _hitBox = new Rectangle(0,0, 192 * 3, 96 * 3 );
        
        _punchHitBox = new Rectangle(worldPos.Center.X, worldPos.Y, 250, 300);

        //Sprite effect
        _spriteEffects = SpriteEffects.None;

        //Skill
        _iceSpike = new IceSpike(iceSpike, new Rectangle(_groundBox.X, _groundBox.Y + _groundBox.Height - 48, _groundBox.Width, 48), player);

        _spawnSpike = _iceSpike.Spawn;
        
        //Delegate
        _checkAnimFinish = _animator.CheckAnimationFinish;
        
        _checkOnGround = SetOnGround;

        _spawnBullet = SpawnBullet;

        _setAnim = SetAnim;

        _getMaxIndex = _animator.GetMaxIndex;

        //Game state
        manager.OnStateChange += OnStateChange;

        _changeState = manager.ChangeState;
    }



    public override void Update(GameTime gameTime)
    {
        if (!visible || paused || !enabled)
        {
            return;
        }

        _animator.Update(gameTime);

        _iceSpike.Update(gameTime);

        worldPos.X += (int)_velocity.X;

        worldPos.Y += (int)_velocity.Y;

        foreach (Bullet bullet in _objectPooling.GetBullets(ObjectPooling.ProjectileType.BossBullet))
        {
            if (bullet.Enabled)
            {
                bullet.Update(gameTime);
            }
        }

        ChangeColorEffect(gameTime);


        if(_dying)
        {
            if(_animator.CheckAnimationFinish(State.Die, _animator.GetMaxIndex(State.Die) - 1))
            {
                this.SetActive(false);
                _changeState(GameManager.GameState.GameOver);
            }
        }else
        {
            StateUpdate();

            UpdateHitBox();

            if(_triggered)
            {
                GenerateCommand(gameTime);

                ProcessCommand(gameTime);
            }
        }
        
    }

    public void Draw(SpriteBatch sb)
    {
        if(!enabled || !visible)
        {
            return;
        }

        foreach (Bullet bullet in _objectPooling.GetBullets(ObjectPooling.ProjectileType.BossBullet))
        {
            bullet.Draw(sb, isDebug, _player.CameraOffset, bullet.Rotation, 2);
        }

        displayPos = new Rectangle(worldPos.X + (int)_player.CameraOffset.X, worldPos.Y + (int)_player.CameraOffset.Y, worldPos.Width,
            worldPos.Height);

        Rectangle groundBoxDisplayPos = new Rectangle(_groundBox.X + (int)_player.CameraOffset.X, _groundBox.Y + (int)_player.CameraOffset.Y, 
            _groundBox.Width, _groundBox.Height);

        Rectangle hitBoxDisplayPos = new Rectangle(_hitBox.X + (int)_player.CameraOffset.X, _hitBox.Y + (int)_player.CameraOffset.Y, 
            _hitBox.Width, _hitBox.Height);

        Rectangle punchHitBox = new Rectangle(worldPos.Center.X + (int)_player.CameraOffset.X, WorldPos.Y + (int)_player.CameraOffset.Y,
            _punchHitBox.Width, _punchHitBox.Height);

        if (!LockDirection) //Avoid change direction when using skill
        {
            _spriteEffects = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None; //Sprite face left by default;
        }

        _iceSpike.Draw(sb, _player.CameraOffset, isDebug);

        if (_changeColorGetHit)
        {
            _animator.Draw(sb, displayPos, _spriteEffects ,Color.Red);
        }
        else
        {
            _animator.Draw(sb, displayPos, _spriteEffects, Color.White);

        }        

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(HitBox.Center.X, HitBox.Center.Y), _radius, 3, Color.Beige);
            CustomDebug.DrawWireRectangle(sb, worldPos, 2, Color.BlueViolet);
            CustomDebug.DrawWireRectangle(sb, punchHitBox, 2, Color.Black);
            CustomDebug.DrawWireRectangle(sb, groundBoxDisplayPos, 5, Color.Cyan);
            CustomDebug.DrawWireRectangle(sb, hitBoxDisplayPos, 2, Color.Red);
        }
    }

    /// <summary>
    /// Apply changes when state changed
    /// </summary>
    /// <param name="state">new state</param>
    private void OnStateChange(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:
                paused = false;
                visible = false;
                isDebug = false;
                break;

            case GameManager.GameState.Reset:
                Respawn();
                break;
            case GameManager.GameState.Game:
                visible = true;
                paused = false;
                isDebug = false;

                break;

            case GameManager.GameState.GameOver:
                visible = false;
                paused = false;
                isDebug = false;

                break;
            case GameManager.GameState.Die:
                visible = false;
                paused = true;
                break;
            case GameManager.GameState.Pause:
                paused = true;

                break;
            case GameManager.GameState.Debug:
                paused = false;
                visible = true;
                isDebug = true;
                break;
        }
    }
    private void StateUpdate()
    { 
        if(_player.WorldPos.X < _hitBox.X)
        {
            _faceRight = false;
        }else if(_player.WorldPos.X > _hitBox.X + _hitBox.Width)
        {
            _faceRight = true;
        }

        //Triggered the initial logic, when player fall into boss zone
        if (!_triggered && _groundBox.Contains(_player.WorldPos))
         {
            if(OnBossFightEvent != null)
            {
                OnBossFightEvent();
            }

            _audioManager.PlaySong(AudioManager.SongID.BossSong);

            _animator.SetState(State.Revive);
            _animator.SetState(State.Idle);
            _currentState = State.Idle;
            _curCommand = null;
            _triggered = true;
            _isFree = true;
        }
       

    }

    /// <summary>
    /// Reset stats when play again
    /// </summary>
    private void Respawn()
    {
        curHP = maxHp;
        _triggered = false;
        _commandQueue.Clear();
        _curCommand = null;
        _isFree = true;
        _dying = false;
        SetActive(true);
        _objectPooling.ClearType(ObjectPooling.ProjectileType.BossBullet);
        HPChanged(curHP);
    }

    /// <summary>
    /// Process command of action in command queue
    /// </summary>
    /// <param name="gameTime"></param>
    private void ProcessCommand(GameTime gameTime)
    {
        //Allows delay time between command executions
        if(_isFree)
        {
            _sinceLastCommand += (float)gameTime.TotalGameTime.TotalSeconds;
        }

        //Execute command
        if (_curCommand != null && _isFree && _sinceLastCommand > _processCommandTimeGap)
        {
            _curCommand.Execute(this);
            _isFree = false;
            _sinceLastCommand = 0;
        }

        //Update command until it finishes
        if(_curCommand != null)
        {
           _curCommand.Update(this, gameTime);

            if(_curCommand.Finished)
            {
                _isFree = true;
                _currentState = State.Idle;
                _animator.SetState(_currentState);
                _curCommand = null;
            }
        }

    }

    /// <summary>
    /// Genrate command automatically after time
    /// </summary>
    /// <param name="gameTime"></param>
    private void GenerateCommand(GameTime gameTime)
    {
        if (_curCommand == null || _curCommand is GetHitCommand) 
        {
            _timeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (_timeCounter <= 0)
        {
            ICommand<Boss> command = GenerateSkill();
            
            if(command is not GetHitCommand)
            {
                _hasGetHitFrame = false;
            }
            if (command != null)
            {
                _commandQueue.Enqueue(command);
            }

            if (_commandQueue.Count > 0)
            {
                _curCommand = _commandQueue.Dequeue();
            }

            _timeCounter = _generateCommandTime;
        }
    }

    /// <summary>
    /// Update hitbox follow wolrd position of the boss
    /// </summary>
    private void UpdateHitBox()
    {
        _hitBox.X = worldPos.Center.X - worldPos.Width / 8;
        _hitBox.Y = worldPos.Y;
        _hitBox.Width = worldPos.Width / 4;
        _hitBox.Height = worldPos.Height;
    }


    /// <summary>
    /// Generate randomly skill (command) in skill set with ratio between them
    /// </summary>
    /// <returns></returns>
    private ICommand<Boss> GenerateSkill()
    {
        //Add sume of all rate
        int sum = 0;
        foreach (Skill<State> skill in _skillSet)
        {
            sum += skill.Rate;
        }
        
        int randomRate = _random.Next(0,sum +1);

        int accum = 0;
        
        //Pick randomly base on ratio, after each iterations, increase the edge to choose
        for (int i = 0; i < _skillSet.Count; i++)
        {
            accum += _skillSet[i].Rate;
            if (accum > randomRate)
            {
                switch (_skillSet[i].State)
                {
                    case State.Spike:
                        return new SpikeCommand(_playSFX,_spawnBullet, _setAnim, _checkAnimFinish, _spikeNumb, _radius, _getMaxIndex(State.Spike) - 1);
                    
                    case State.Jump:
                        return new JumpCommand(_playSFX,_spawnSpike, _setAnim, _getMaxIndex, _jumpForce, _gravity, _checkAnimFinish, _checkOnGround);

                    case State.Punch:
                        return new PunchCommand(_playSFX,_speed, _skillSet[i].Damage, _getMaxIndex(State.Punch) - 1,_punchHitBox, _player, _setAnim,
                            _checkAnimFinish);
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// Spawn bullet (ice spike), used mainly by a SpikeCommand
    /// </summary>
    /// <param name="direction">direction of bullet</param>
    /// <param name="spawnPos">spawn position of bullet</param>
    /// <param name="radAngle">angle of direction (along x-axis) to rotate the texture</param>
    private void SpawnBullet(Vector2 direction, Vector2 spawnPos, float radAngle)
    {
        Bullet bullet = _objectPooling.GetObj(ObjectPooling.ProjectileType.BossBullet, _bulletTexture, _level);
        bullet.Spawn(Constants.BossBulletSpriteMap,_player,radAngle,spawnPos, direction, _bulletDamge, _bulletSpeed, 48,32, 1,0.05f, _bulletHitBox);
    }

    /// <summary>
    /// Set animation of the boss
    /// </summary>
    /// <param name="state">boss's state</param>
    private void SetAnim(State state)
    {
        _currentState = state;
        _animator.SetState(_currentState);
    }

    /// <summary>
    /// Get hit logic, handle get animation, and wait for animation of die before disabled
    /// </summary>
    /// <param name="damage"></param>
    public override void GetHit(int damage)
    {
        curHP = MathHelper.Clamp(curHP - damage, 0, maxHp);
        HPChanged(curHP);

        if (Free && !_hasGetHitFrame)
        {
            GetHitCommand takeHit = new GetHitCommand(_setAnim, _checkAnimFinish, _getMaxIndex);
            _commandQueue.Enqueue(takeHit);
            _hasGetHitFrame = true;
        }

        if(curHP <= 0)
        {
            _animator.SetState(State.Die);
            _dying = true;
        }


        _changeColorGetHit = true;
    }

    /// <summary>
    /// Handle change color effect when get hit
    /// </summary>
    /// <param name="gameTime"></param>
    private void ChangeColorEffect(GameTime gameTime)
    {
        if (_changeColorGetHit)
        {
            _sinceChangeColor += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_sinceChangeColor > _changeColorTime)
            {
                _sinceChangeColor = 0;
                _changeColorGetHit = false;
            }
        }

    }

    /// <summary>
    /// Remap boss into a box, avoid fall out
    /// </summary>
    /// <returns></returns>
    private bool SetOnGround()
    {
        if(HitBox.X < _groundBox.X)
        {
            this.worldPos.X += _groundBox.X - HitBox.X;
        }

        if(HitBox.X + HitBox.Width > _groundBox.X + _groundBox.Width)
        {
            this.worldPos.X -= HitBox.X + HitBox.Width - (_groundBox.X + _groundBox.Width);
        }

        if (HitBox.Y + HitBox.Height > _groundBox.Y + _groundBox.Height)
        {
            this.worldPos.Y -= HitBox.Y + HitBox.Height - (_groundBox.Y + _groundBox.Height);
            return true;
        }
        return false;
    }
}

