using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;

public class ObjectPooling
{
    public enum ProjectileType
    {
        Bullet,
        BossBullet,
    }

    private static ObjectPooling _instance;
    
    private Dictionary<ProjectileType, List<Bullet>> _pool;

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

    public List<Bullet> GetBullets(ProjectileType type)
    {
        if (_pool.ContainsKey(type))
        {
            return _pool[type];
        }
        return new List<Bullet>();
    }

    public int GetBulletCount(ProjectileType type)
    {
        if (_pool.ContainsKey(ProjectileType.BossBullet))
        {
            return _pool[ProjectileType.BossBullet].Count;
        }
        return -1;
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