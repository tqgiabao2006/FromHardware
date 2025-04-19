using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{

    public enum EnemyType
    {
        Ground,
        Fly
    }

    internal class EnemySpawner
    {
        private Dictionary<EnemyType, Enemy> _posMap;

        private List<Enemy> _enemyList;

        private Texture2D _groundSpriteSheet;

        private Texture2D _flySpriteSheet;

        private Level _level;

        private Player _player;

        private GameManager _gameManager;

        public Player Player { set { _player = value; } }

        public GameManager GameManager {  set { _gameManager = value; } }

        public List<Enemy> EnemyList { get { return _enemyList; } }

        public EnemySpawner(Texture2D groundSpriteSheet, Texture2D flySpriteSheet, string filePath, Level level)
        {
            _groundSpriteSheet = groundSpriteSheet;

            _flySpriteSheet = flySpriteSheet;

            _posMap = new Dictionary<EnemyType, Enemy>();

            _enemyList = new List<Enemy>();

            _level = level;

            LoadPosition(filePath);
        }

        private void Draw(SpriteBatch sb)
        {
            foreach (Enemy enemy in _enemyList)
            {
                enemy.Draw(sb);
            }
        }

        private void LoadPosition(string filepath)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filepath);
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    //Skip the line start by '/' or '=' because it is description, not data
                    if (line[0] == '/' || line[0] == '=')
                    {
                        continue;
                    }

                    string[] data = line.Split(',');
                    EnemyType type = (EnemyType)Enum.Parse(typeof(EnemyType), data[0]);
                    Rectangle leftBound = _level[int.Parse(data[1]), int.Parse(data[2])].WorldPos;
                    Rectangle rightBound = _level[int.Parse(data[3]), int.Parse(data[4])].WorldPos;
                    Enemy enemy = new Enemy(_groundSpriteSheet, rightBound, leftBound, _player, _gameManager);

                    _posMap.Add(type, enemy);
                    _enemyList.Add(enemy);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
