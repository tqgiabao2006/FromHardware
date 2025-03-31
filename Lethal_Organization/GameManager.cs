using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal class GameManager
    {
        private static GameManager _instance;
        
        public enum GameState
        {
            Menu,
            Game,
            Pause,
            GameOver
        }
        
        public event Action<GameState> StateChangedAction = delegate { };
        private GameState _currentState;

        public GameState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                StateChangedAction(_currentState);
            }
        }

        private GameManager()
        {

        }

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
