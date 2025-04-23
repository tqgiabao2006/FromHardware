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
        private List<Enemy> _enemyList;

        private Texture2D _groundSpriteSheet;

        private Texture2D _flySpriteSheet;

        private Texture2D _UISpriteSheet;

        private Rectangle _HPSourceImg;

        private Level _level;

        private Player _player;

        private GameManager _gameManager;

        public List<Enemy> EnemyList { 
            get 
            { 
                return _enemyList;
            } 
        }

        public EnemySpawner(Texture2D groundSpriteSheet, Texture2D flySpriteSheet, Texture2D UISpriteSheet, Rectangle EnemyHPSourceImg,string filePath, Level level, GameManager gameManager, Player player)
        {
            _groundSpriteSheet = groundSpriteSheet;

            _flySpriteSheet = flySpriteSheet;

            _UISpriteSheet = UISpriteSheet;

            _HPSourceImg = EnemyHPSourceImg;

            _enemyList = new List<Enemy>();

            _level = level;

            _gameManager = gameManager;

            _player = player;

            LoadPosition(filePath);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Enemy enemy in _enemyList)
            {
                enemy.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Enemy enemy in _enemyList)
            {
                enemy.Draw(sb, _player);
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
                    Vector2 leftBound = new Vector2(48 * int.Parse(data[1]), 48 * int.Parse(data[2]));
                    Vector2 rightBound = new Vector2(48 * int.Parse(data[3]), 48 * int.Parse(data[4])); //Each tile of the the map 16 x 3 = 48

                    if(type == EnemyType.Ground)
                    {
                        Enemy enemy = new Enemy(_groundSpriteSheet, _UISpriteSheet, _HPSourceImg, rightBound, leftBound, _player, _gameManager, 57, 42,2);
                        _enemyList.Add(enemy);
                    }
                    else if(type == EnemyType.Fly) 
                    {
                        Enemy enemy = new Enemy(_flySpriteSheet, _UISpriteSheet, _HPSourceImg, rightBound, leftBound, _player, _gameManager, 83, 64,3);
                        _enemyList.Add(enemy);
                    }               
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
