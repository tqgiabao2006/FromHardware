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
    public class GameManager: IDrawable
    {
        public enum GameState
        {
            Menu,
            Game,
            Debug,
            Pause,
            GameOver
        }

        public event Action<GameState> StateChangedAction = delegate { };

        private GameState _currentState;

        private KeyboardState _kbState;

        private KeyboardState _prevKbState;

        private SpriteFont _font;

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

        public GameManager(SpriteFont font)
        {
            _font = font;
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
                    if (IsSinglePressed(Keys.Enter))
                    {
                        CurrentState = GameState.Game;
                    }
                    break;

                case GameState.Game:
                    if (IsSinglePressed(Keys.Q))
                    {
                        CurrentState = GameState.Pause;
                    }
                    if (IsSinglePressed(Keys.R))
                    {
                        CurrentState = GameState.Debug;
                    }
                    if (IsSinglePressed(Keys.P))
                    {
                        CurrentState = GameState.GameOver;
                    }
                    break;

                case GameState.Pause:
                    if (IsSinglePressed(Keys.Q))
                    {
                        CurrentState = GameState.Game;
                    }
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
                    if (IsSinglePressed(Keys.Q))
                    {
                        CurrentState = GameState.Pause;
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

        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(_font,
               $"{_currentState}",
               new Vector2(100, 100),
               Color.White);
        }
    }
}
