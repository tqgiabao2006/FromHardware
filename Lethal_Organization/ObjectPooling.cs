using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class ObjectPooling
{
    public enum ProjectileType
    {
        Bullet,
    }

    private static ObjectPooling _instance;
    
    private Dictionary<ProjectileType, List<Bullet>> _pool;

    public List<Bullet> Bullets
    {
        get
        {
            if (_pool.ContainsKey(ProjectileType.Bullet))
            {
                return _pool[ProjectileType.Bullet];
            }
            return new List<Bullet>();
        }
    }
    public int BulletCount
    {
        get
        {
            if (_pool.ContainsKey(ProjectileType.Bullet))
            {
                return _pool[ProjectileType.Bullet].Count;
            }
            return -1;
        }
    }

    public static ObjectPooling Instance
    {
        get
        {
            if (_instance == null)
            {
                return new ObjectPooling();
            }
            return _instance;
        }
    }
    
    private ObjectPooling()
    {
        _pool = new Dictionary<ProjectileType, List<Bullet>>();
    }

    public Bullet GetObj(ProjectileType type, Texture2D texture, Level level)
    {
        List<Bullet> listObject = new List<Bullet>();

        if (_pool.ContainsKey(type))
        {
            listObject =  _pool[type];
        }
        else
        {
            _pool.Add(type, listObject);
        }

        
        foreach (Bullet obj in listObject)
        {
            if (obj.Enabled)
            {
                continue;
            }
            else
            {
                return obj;
            }
        }
        
        //If not found any inactive object to reuse craft a new oe
        Bullet bullet = new Bullet(texture, level);
        _pool[type].Add(bullet);
        return bullet;
    }
}