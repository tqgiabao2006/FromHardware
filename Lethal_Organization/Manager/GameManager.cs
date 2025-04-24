using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal class GameManager
    {

        public enum GameState
        {
            Menu,
            Game,
            Debug,
            Pause,
            Die,
            GameOver,
            Reset,
        }

        public event Action<GameState> OnStateChange = delegate { };

        private GameState _currentState;

        private KeyboardState _kbState;

        private KeyboardState _prevKbState;
        
        /// <summary>
        /// Store any object reacts to change of the state
        /// </summary>
        public Action<GameState> ChangeState;

        public GameState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                OnStateChange(_currentState);
            }
        }
        public GameManager()
        {
            ChangeState = SetState;
        }

        public void Start()
        {
            CurrentState = GameState.Menu;
        }

        public void Update(GameTime gameTime)
        {
            _kbState = Keyboard.GetState();

            switch(_currentState)
            {
                case GameState.Menu:
                    break;

                case GameState.Game:
                    if (IsSinglePressed(Keys.R))
                    {
                        CurrentState = GameState.Debug;
                    }
                    break;

                case GameState.Pause:
                    if (IsSinglePressed(Keys.R))
                    {
                        CurrentState = GameState.Debug;
                    }
                    break;

                case GameState.Debug:
                    if (IsSinglePressed(Keys.R))
                    {
                        CurrentState = GameState.Game;
                    }
                    break;
                case GameState.Die:
                    if (IsSinglePressed(Keys.Enter))
                    {
                        CurrentState = GameState.Reset;
                        CurrentState = GameState.Game;
                    }
                    break;
                case GameState.GameOver:
                    if (IsSinglePressed(Keys.Enter))
                    {
                        CurrentState = GameState.Menu;
                    }

                    break;

            }
            _prevKbState = Keyboard.GetState();
        }

        /// <summary>
        /// Check if a key is single pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsSinglePressed(Keys key)
        {
            return (_kbState.IsKeyDown(key) && _prevKbState.IsKeyUp(key));
        }

        /// <summary>
        /// Set state of the game
        /// </summary>
        /// <param name="state"></param>
        private void SetState(GameState state)
        {
            CurrentState = state;
        }
    }
}
