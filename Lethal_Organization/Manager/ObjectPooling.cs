using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization;


/// <summary>
/// Object Pooling pattern to store and resuse projectile
/// </summary>
internal class ObjectPooling
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

    /// <summary>
    /// Get bullet list base on given type
    /// </summary>
    /// <param name="type">Projectile Type </param>
    /// <returns></returns>
    public List<Bullet> GetBullets(ProjectileType type)
    {
        if (_pool.ContainsKey(type))
        {
            return _pool[type];
        }
        return new List<Bullet>();
    }

    /// <summary>
    /// Get count of list of given type
    /// </summary>
    /// <param name="type">Projectile Type </param>
    /// <returns></returns>
    public int GetBulletCount(ProjectileType type)
    {
        if (_pool.ContainsKey(ProjectileType.BossBullet))
        {
            return _pool[ProjectileType.BossBullet].Count;
        }
        return -1;
    }

    /// <summary>
    /// Create or resuse the object needed to spawn
    /// </summary>
    /// <param name="type">Type of projectile</param>
    /// <param name="texture">Texture of projectype</param>
    /// <param name="level">Level it fly on (main level)</param>
    /// <returns></returns>
    public Bullet GetObj(ProjectileType type, Texture2D texture, Level level)
    {
        List<Bullet> listObject = new List<Bullet>();

        //If already has list in pool => use it
        if (_pool.ContainsKey(type))
        {
            listObject =  _pool[type];
        }
        else // if not creat new
        {
            _pool.Add(type, listObject);
        }


        //Get the unactive object to respawn
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
        
        //If not found any inactive object to reuse craft a new one
        Bullet bullet = new Bullet(texture, level);
        _pool[type].Add(bullet);
        return bullet;
    }
  
    /// <summary>
    /// Reset pool when the game reset
    /// </summary>
    /// <param name="type"></param>
    public void ClearType(ProjectileType type)
    {
        if(!_pool.ContainsKey(type))
        {
            return;
        }

        foreach(Bullet obj in _pool[type])
        {
            obj.SetActive(false);
        }
    }
}