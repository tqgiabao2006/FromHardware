
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lethal_Organization;

public class Bullet
{
    public enum Direction
    {
        Left,
        Right
    }
    
    private bool _enabled;

    private int _speed;
    
    private int _hitBoxRadius;

    private int _range;

    private int _damge;
    
    private Direction _direction;

    private Level _level;
    
    // Asset
    private Texture2D _texture;
    
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

    //Debug-only
    public int Speed
    {
        get
        {
            return _speed;
        }
    }

    public Bullet(Texture2D texture, Level level)
    {
        this._level = level;
        this._hitBoxRadius = 10;
        this._range = 1000;
        this._damge = 10;
        this._speed = 10;
        this.SetActive(false);
        this._texture = texture;
        this._sourceImg = new Rectangle(0, 0, 16, 16);
        this._worldPos = new Rectangle(0, 0, 16, 16);
    }

    public void Spawn(Vector2 worldPos, Direction direction)
    {
        _spawnPos = worldPos;
        this._worldPos.X= (int)worldPos.X;
        this._worldPos.Y = (int)worldPos.Y;
        this._enabled = true;
        this._direction = direction;
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
                        SetActive(false);
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

        CheckCollision();

        if (_direction == Direction.Right)
        {
            if (this._worldPos.X > this._spawnPos.X + this._range)
            {
                SetActive(false);
                return;
            }

            this._worldPos.X += (_speed);
        }
        else
        {
            if (this._worldPos.X < this._spawnPos.X - this._range)
            {
                SetActive(false);
                return;
            }
            
            this._worldPos.X -= _speed;
        }
    }
      
    public void Draw(SpriteBatch sb, bool isDebug, Vector2 cameraOffset)
    {
        _displayPos = new Rectangle(_worldPos.X + (int)cameraOffset.X,(int)_spawnPos.Y + (int)cameraOffset.Y, _worldPos.Width, _worldPos.Height); //Lock y axis

        SpriteEffects effect = _direction == Direction.Right? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        if (_enabled)
        {
            sb.Draw(_texture, _displayPos, _sourceImg,Color.White, 0, Vector2.Zero, effect, 0);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(_worldPos.X, _worldPos.Y),  _hitBoxRadius, 3, Color.Red);
        }
    }
}