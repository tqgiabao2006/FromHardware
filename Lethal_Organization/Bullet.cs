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
    
    protected bool enabled;

    protected int speed;
    
    protected int hitBoxRadius;

    protected int range;

    protected int damge;
    
    protected Direction direction;

    protected Level level;
    
    // Asset
    protected Texture2D texture;
    
    //Positions
    protected Rectangle displayPos; //For display only
              
    protected Rectangle sourceImg; //For render from sprite sheet
            
    protected Rectangle worldPos; //Real position to do with logic

    protected Vector2 spawnPos;
    
    public bool Enabled
    {
        get
        {
            return enabled;
        }
    }
    
    public Vector2 WorldPos
    {
        get
        {
            return new Vector2(worldPos.X, worldPos.Y); 
        }
        private set
        {
            worldPos.X = (int)value.X;
            worldPos.Y = (int)value.Y;
        }
    }

    //Debug-only
    public int Speed
    {
        get
        {
            return speed;
        }
    }

    public Bullet(Texture2D texture, Level level)
    {
        this.level = level;
        this.hitBoxRadius = 50;
        this.range = 1000;
        this.damge = 10;
        this.speed = 5;
        this.SetActive(false);
        this.texture = texture;
        this.sourceImg = new Rectangle(0, 0, 16, 16);
        this.worldPos = new Rectangle(0, 0, 16, 16);
    }

    public void Spawn(Vector2 worldPos, Direction direction)
    {
        spawnPos = worldPos;
        this.worldPos.X= (int)worldPos.X;
        this.worldPos.Y = (int)worldPos.Y;
        this.enabled = true;
        this.direction = direction;
    }

    public void SetActive(bool active)
    {
        enabled = active;
    }

    public void Collide()
    {
        if (level == null)
        {
            return;
        }

        for (int i = 0; i < level.SizeX; i++)
        {
            for (int j = 0; j < level.SizeY; j++)
            {
                Rectangle tilePos = level[i, j].WorldPos;
                if (Collide(new Vector2(tilePos.X, tilePos.Y), tilePos.Width / 2))
                {
                    SetActive(false);
                    return;
                }
            }
        }
    }

    private bool Collide(Vector2 otherCenter, float otherRadius)
    {
        float sqrDst = Vector2.DistanceSquared(otherCenter,  WorldPos);
        
        return sqrDst < otherRadius + otherRadius;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!enabled)
        {
            return;
        }
        
        if (direction == Direction.Right)
        {
            if (this.worldPos.X > this.spawnPos.X + this.range)
            {
                SetActive(false);
                return;
            }

            this.worldPos.X += speed;
        }
        else
        {
            if (this.worldPos.X < this.spawnPos.X - this.range)
            {
                SetActive(false);
                return;
            }
            
            this.worldPos.X -= speed;
        }
    }
      
    public void Draw(SpriteBatch sb, bool isDebug)
    {
        if (enabled)
        {
            sb.Draw(texture, worldPos, sourceImg, Color.White);
        }

        if (isDebug)
        {
            CustomDebug.DrawWireCircle(sb, new Vector2(worldPos.X, worldPos.Y),  hitBoxRadius, 3, Color.Red);
            
        }
    }
}