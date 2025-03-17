using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{

    public enum GameState
    {

    }

    internal class GameManager
    {
        private static GameManager _instance;
        private static readonly object _lock = new object();

        private GameManager() { }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new GameManager();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
