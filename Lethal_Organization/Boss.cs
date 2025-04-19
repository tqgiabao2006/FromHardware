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

    private Player _player;
    
    private Level _level;
    
    private Random _random;
    
    private ObjectPooling _objectPooling;

    private Spike _spike;
    
    private Animator<State> _animator;

    private List<Skill<State>> _skillSet;

    private Queue<ICommand<Boss>> _commandsQueue;
    
    //Asset
    private Texture2D _bulletTexture;
    
    //Delegate
    private Action<Vector2, Vector2, float> _spawnBullet;

    private Action<State> _setAnim;

    private Func<bool> _checkOnGround;

    private Func<State, int,bool> _checkAnimFinish;

    private Func<State, int> _getMaxIndex;
    
    //Skill
    private bool _triggered;

    private Rectangle _triggerBound;

    private Rectangle _punchHitBox;

    private Rectangle _hitBox;

    private Rectangle _groundBox;

    private int _phase2Hp;

    private float _delayTime;

    private float _timeCounter;

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

    //Debug
    public bool OnCommand
    {
        get { return _curCommand != null && !_curCommand.Finished; }
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
    
    public Boss(Texture2D spriteSheet, Texture2D bulletTexture,string textureMapFile, 
        Player player, Level level,
        GameManager manager, Random random, ObjectPooling objectPooling)
    {
        //Class
        this._player = player;
        
        this._level = level;    
        
        this._currentState = State.Die;
        
        texture = spriteSheet;
        
        _bulletTexture = bulletTexture;

        _animator = new Animator<State>(spriteSheet, _currentState, textureMapFile, 0.1f);

        _skillSet = new List<Skill<State>>()
        {
            new Skill<State>
            {
                State = State.Punch,
                Rate = 50,
                Damage = 20
            },
            new Skill<State>
            {
                State = State.Spike,
                Rate = 30,
                Damage = 20
            }
        };
        
        _commandQueue = new Queue<ICommand<Boss>>();
        
        this._random = random;
        
        _objectPooling = objectPooling;
        
        //Movement
        hp = 1000;

        _phase2Hp = 1000 / 2;

        _delayTime = 8;

        _timeCounter = 3;
        
        _velocity = new Vector2(0, 0);

        _speed = 2;

        _jumpForce = new Vector2(_speed * 3, -10);

        _gravity = 0.2f;

        //Shoot
        _spikeNumb = 10;

        _radius = 300;

        _bulletHitBox = 20;

        _bulletRange = 1500;

        _bulletSpeed = 8;

        _bulletDamge = 10;

        //Position_HitBox
        worldPos = new Rectangle(0,200, 192, 96);

        _hitBox = new Rectangle(0, 200, 100, 96);
        
        _punchHitBox = new Rectangle(0, 200, 300, 64);

        _groundBox = new Rectangle(0, 300, 1000, 1009);

        _triggerBound = new Rectangle(0, 200, 300, 300);


        //Sprite effect
        _spriteEffects = SpriteEffects.None;

        //Delegate
        _checkAnimFinish = _animator.CheckAnimationFinish;
        
        _checkOnGround = SetOnGround;

        _spawnBullet = SpawnBullet;

        _setAnim = SetAnim;

        _getMaxIndex = _animator.GetMaxIndex;

        //Game state
        manager.StateChangedAction += OnStateChange;

        isDebug = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!visible || paused)
        {
            return;
        }

        _animator.Update(gameTime);

        worldPos.X += (int)_velocity.X;

        worldPos.Y += (int)_velocity.Y;

        foreach (Bullet bullet in _objectPooling.GetBullets(ObjectPooling.ProjectileType.BossBullet))
        {
            if (bullet.Enabled)
            {
                bullet.Update(gameTime);
            }
        }

        UpdateHitBox();

        StateUpdate();

        GenerateCommand(gameTime);

        ProcessCommand(gameTime);
    }

    public void Draw(SpriteBatch sb)
    {
        foreach (Bullet bullet in _objectPooling.GetBullets(ObjectPooling.ProjectileType.BossBullet))
        {
            bullet.Draw(sb, isDebug, _player.CameraOffset, bullet.Rotation);
        }

        displayPos = new Rectangle(worldPos.X + (int)_player.CameraOffset.X, worldPos.Y + (int)_player.CameraOffset.Y, worldPos.Width, worldPos.Height);

        if (!LockDirection) //Avoid change direction when using skill
        {
            _spriteEffects = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None; //Sprite face left by default;
        } 

        if(visible)
        {
            _animator.Draw(sb, worldPos, _spriteEffects);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(HitBox.Center.X, HitBox.Center.Y), _radius, 3, Color.Beige);
            CustomDebug.DrawWireRectangle(sb, worldPos, 2, Color.BlueViolet);
          
            CustomDebug.DrawWireRectangle(sb, _groundBox, 5, Color.Cyan);
            CustomDebug.DrawWireRectangle(sb, _hitBox, 2, Color.Yellow);
        }
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
                visible = true;
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
                visible = true;
                isDebug = false;

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

        if (!_triggered && _triggerBound.Contains(_player.WorldPos))
         {
            _animator.SetState(State.Revive);
            _animator.SetState(State.Idle);
            _currentState = State.Idle;
            _curCommand = null;
            _triggered = true;
            _isFree = true;
        }
        else
        {
            if (hp < _phase2Hp)
            {
                //Add new skill
                _skillSet.Add(new Skill<State>()
                {
                    State = State.Jump,
                    Damage = 50,
                    Rate = 20,
                });
            }
        }

    }

    private void ProcessCommand(GameTime gameTime)
    {
        if (_curCommand != null && _isFree)
        {
            _curCommand.Execute(this);
            _isFree = false;
        }

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

    private void GenerateCommand(GameTime gameTime)
    {
        if (_curCommand == null) 
        {
            _timeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (_timeCounter <= 0)
        {
            ICommand<Boss> command = GenerateSkill();
            if (command != null)
            {
                _commandQueue.Enqueue(command);
            }

            if (_commandQueue.Count > 0)
            {
                _curCommand = _commandQueue.Dequeue();
            }

            _timeCounter = _delayTime;
        }
    }

    private void UpdateHitBox()
    {
        _hitBox.X = worldPos.Center.X - worldPos.Width / 4;
        _hitBox.Y = worldPos.Y;
        _hitBox.Width = worldPos.Width / 2;
        _hitBox.Height = worldPos.Height;
    }

    private ICommand<Boss> GenerateSkill()
    {
        return new JumpCommand(_setAnim, _getMaxIndex,_jumpForce, _gravity, _checkAnimFinish, _checkOnGround);

        return new SpikeCommand(_spawnBullet, _setAnim, _checkAnimFinish, _spikeNumb, _radius, _getMaxIndex(State.Spike) - 1);

        int sum = 0;
        foreach (Skill<State> skill in _skillSet)
        {
            sum += skill.Rate;
        }
        
        int randomRate = _random.Next(sum +1);

        int accum = 0;
        
        for (int i = 0; i < _skillSet.Count; i++)
        {
            accum += _skillSet[i].Rate;
            if (accum > randomRate)
            {
                switch (_skillSet[i].State)
                {
                    case State.Spike:
                        return new SpikeCommand(_spawnBullet, _setAnim, _checkAnimFinish, _spikeNumb, _radius, _getMaxIndex(State.Spike) - 1);
                    
                    case State.Jump:
                        return new JumpCommand(_setAnim, _getMaxIndex, _jumpForce, _gravity, _checkAnimFinish, _checkOnGround);
                    
                    case State.Punch:
                        return new PunchCommand(_speed, _skillSet[i].Damage, _getMaxIndex(State.Punch) - 1,_punchHitBox, _player, _setAnim,
                            _checkAnimFinish);
                }
            }
            return null;
        }
        
        return null;
    }

    private void SpawnBullet(Vector2 direction, Vector2 spawnPos, float radAngle)
    {
        Bullet bullet = _objectPooling.GetObj(ObjectPooling.ProjectileType.BossBullet, _bulletTexture, _level);
        bullet.Spawn(Constants.BossBulletSpriteMap,radAngle,spawnPos, direction, _bulletDamge, _bulletSpeed, 48,32, 0.05f, _bulletHitBox);
    }

    private void SetAnim(State state)
    {
        _currentState = state;
        _animator.SetState(_currentState);
    }

    private bool SetOnGround()
    {
        if (_hitBox.Intersects(_groundBox))
        {
            Rectangle collidedObject = Rectangle.Intersect(_hitBox, _groundBox);
            this.worldPos.Y -= collidedObject.Height;
            return true;
        }
        return false;
    }
}

