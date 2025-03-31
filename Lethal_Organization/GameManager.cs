using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{

    public enum GameState
    {
        Menu,
        Game,
        Pause,
        GameOver
    }

    internal class GameManager
    {
        private static GameManager _instance;

        private GameManager() { }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                { 
                   _instance = new GameManager();
                }
                return _instance;
            }
        }
    }
}
