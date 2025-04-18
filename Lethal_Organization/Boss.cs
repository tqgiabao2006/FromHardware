using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class Boss: GameObject
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
    private Action<Vector2, Vector2> _spawnBullet;

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
    
    //Command
    private ICommand<Boss> _curCommand;
    
    private Queue<ICommand<Boss>> _commandQueue;

    //Draw
    public bool LockDirection;

    private SpriteEffects _spriteEffects;
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
        
        hp = 1000;

        _phase2Hp = 1000 / 2;

        _delayTime = 3;

        _timeCounter = 3;
        
        _velocity = new Vector2(0, 0);

        _speed = 2;

        _jumpForce = new Vector2(_speed, 10);

        _gravity = 2;

        _spikeNumb = 10;

        _radius = 300;

        worldPos = new Rectangle(0,380, 192, 96);

        _hitBox = new Rectangle(0, 380, 100, 96);
        
        _punchHitBox = new Rectangle(0, 380, 300, 64);

        _groundBox = new Rectangle(0, 450, 1000, 1009);

        _triggerBound = new Rectangle(0, 200, 300, 300);

        _spriteEffects = SpriteEffects.None;

        _checkAnimFinish = _animator.CheckAnimationFinish;
        
        _checkOnGround = SetOnGround;

        _spawnBullet = SpawnBullet;

        _setAnim = SetAnim;

        _getMaxIndex = _animator.GetMaxIndex;

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

        UpdateHitBox();

        StateUpdate();

        GenerateCommand(gameTime);

        ProcessCommand(gameTime);
    }

    public void Draw(SpriteBatch sb)
    {
        displayPos = new Rectangle(worldPos.X + (int)_player.CameraOffset.X, worldPos.Y + (int)_player.CameraOffset.Y, worldPos.Width, worldPos.Height);

        if (!LockDirection) //Avoid change direction when use skill
        {
            _spriteEffects = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None; //Sprite face left by default;
        } 

        _animator.Draw(sb, worldPos, _spriteEffects);

        if(isDebug)
        {
            CustomDebug.DrawWireRectangle(sb, worldPos, 2, Color.BlueViolet);
            CustomDebug.DrawWireRectangle(sb, _punchHitBox, 5, Color.Red);
            CustomDebug.DrawWireRectangle(sb, _groundBox, 2, Color.Yellow);
            CustomDebug.DrawWireRectangle(sb, worldPos, 2, Color.Cyan);
            CustomDebug.DrawWireRectangle(sb, _hitBox, 5, Color.Yellow);
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
            }
        }

    }

    private void GenerateCommand(GameTime gameTime)
    {
        if (_curCommand == null || _curCommand.Finished) 
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
                        return new JumpCommand(_setAnim, _jumpForce, _gravity, _getMaxIndex(State.Jump) - 1, _checkAnimFinish, _checkOnGround);
                    
                    case State.Punch:
                        return new PunchCommand(_speed, _skillSet[i].Damage, _getMaxIndex(State.Punch) - 1,_punchHitBox, _player, _setAnim,
                            _checkAnimFinish);
                }
            }
            return null;
        }
        
        return null;
    }

    private void SpawnBullet(Vector2 direction, Vector2 spawnPos)
    {
        Bullet bullet = _objectPooling.GetObj(ObjectPooling.ProjectileType.Bullet, _bulletTexture, _level);
        bullet.Spawn(spawnPos, direction);
    }

    private void SetAnim(State state)
    {
        _currentState = state;
        _animator.SetState(_currentState);
    }

    private bool SetOnGround()
    {
        if (this.Collides(_groundBox))
        {
            Rectangle collidedObject =this.Collide(_groundBox);
            this.worldPos.Y += collidedObject.Height;
            return true;
        }
        return false;
    }
}

public class PunchCommand : ICommand<Boss>
{
    public bool Finished { set; get; }

    private bool _faceRight;

    private bool _waitAnim;

    private float _speed;

    private float _punchRange;

    private int _damage;

    private int _damgeFrame;

    private int _endFrame;

    private Rectangle _punchHitbox;

    private Player _player;
    
    private Action<Boss.State> _setAnim;
    
    Func<Boss.State,int, bool> _checkFinishAnimation;
    
    internal PunchCommand(float speed, int damage, int endFrame,Rectangle punchHitBox, Player player, Action<Boss.State> setAnim, Func<Boss.State, int,bool> checkFinishAnimation)
    {
        _damgeFrame = 6;

        _endFrame = endFrame;

        _punchRange = punchHitBox.Width;
        
        _punchHitbox = punchHitBox; 
        
        _speed = speed;

        _player = player;
        
        _checkFinishAnimation = checkFinishAnimation;
        
        _damage = damage; 
        
        Finished = false;

        _waitAnim = false;
        
        _setAnim = setAnim;
    }

    public void Execute(Boss boss)
    {
        _setAnim(Boss.State.Idle);
    }

    public void Update(Boss boss, GameTime gameTime)
    {
        if (_waitAnim)
        {
            if (_checkFinishAnimation(Boss.State.Punch,_damgeFrame))
            {
                boss.LockDirection = true;

                if (!_faceRight)
                {
                    _punchHitbox.X = _punchHitbox.X - boss.WorldPos.Width -  _punchHitbox.Width; 
                }
                if (_punchHitbox.Intersects(_player.WorldPos))
                {
                    _player.GetHit(_damage);
                }

            }

            if(_checkFinishAnimation(Boss.State.Punch,_endFrame))
            {
                boss.LockDirection=false;
                Finished = true;
            }

            boss.Velocity = Vector2.Zero;
        }
        else
        {
            if(boss.WorldPos.Intersects(_player.WorldPos))
            {
                _setAnim(Boss.State.Punch);
                _faceRight = _player.WorldPos.X > boss.WorldPos.X;
                _waitAnim = true;
            }else
            {
                _setAnim(Boss.State.Walk);
                
                int dirMultipler = boss.FaceRight ? 1 : -1;

                boss.Velocity = new Vector2(dirMultipler*_speed, 0);

                boss.BossState = Boss.State.Walk;
            }
        }
        
    }
}

public class JumpCommand : ICommand<Boss>
{
    public bool Finished { set; get; }

    private Action<Boss.State> _setAnim;

    private Vector2 _jumpForce;

    private float _gravity;
    
    private Func<Boss.State, int,bool> _checkFinishAnimation;
    
    private Func<bool> _onGround;

    private int _endFrame;
    
    internal JumpCommand(Action<Boss.State> setAnim, Vector2 jumpForce, float gravity, int endFrame,Func<Boss.State, int,bool> checkFinishAnimation, Func<bool> onGround)
    {
        _setAnim = setAnim;
        
        _jumpForce = jumpForce;
        
        _checkFinishAnimation = checkFinishAnimation;
        
        _onGround = onGround;
        
        _gravity = gravity;

        _endFrame = endFrame;
    }
    public void Execute(Boss boss)
    {
        _setAnim(Boss.State.Jump);
        boss.Velocity = new Vector2(boss.Velocity.X, boss.Velocity.Y - _jumpForce.Y);
    }

    public void Update(Boss boss, GameTime gameTime)
    {
        boss.Velocity = new Vector2(boss.Velocity.X, boss.Velocity.Y + _gravity);
        if (boss.Velocity.Y > 0)
        {
            _setAnim(Boss.State.Fall);
        }
        if (_checkFinishAnimation(Boss.State.Fall, _endFrame) && _onGround())
        {
            //Spawn spike
            Finished = true;
        }
    }
}

public class SpikeCommand : ICommand<Boss>
{
    public bool Finished { set; get; }

    private Action<Vector2, Vector2> _spawnBullet;
    
    private Action<Boss.State> _setAnim;
    
    Func<Boss.State, int,bool> _checkFinishAnimation;

    private int _numbSpikes;

    private float _radius;

    private int _endFrame;

    public SpikeCommand(Action<Vector2, Vector2> spawnBullet, Action<Boss.State> setAnim, Func<Boss.State, int, bool> checkFinishAnimation, int numbSpikes, float radius, int endFrame)
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
        if (_checkFinishAnimation(Boss.State.Spike, _endFrame))
        {
            int step = 360 / _numbSpikes;

            for (int i = 0; i < 180; i += step)
            {
                float x = _radius * MathF.Cos(i);
                float y = _radius * MathF.Sin(i);

                Vector2 direction = (new Vector2(x, y) - new Vector2(boss.WorldPos.Center.X, boss.WorldPos.Center.Y));
                direction.Normalize();
                
                _spawnBullet(direction, new Vector2(x,y));
            }

            Finished = true;
        }
    }
}
