using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    internal class Button: IDrawable, IUpdateable
    {
        private Texture2D _menuButtonTexture;
        private Texture2D _smollButtonTexture;
        private Vector2 _position;
        private Rectangle _bounds;

        private Color _defaultColor = Color.White;
        private Color _hoveredColor = Color.Gray;
        private Color _clickedColor = Color.DarkGray;

        private bool _isHovered;
        private bool _isPressed;

        private Rectangle _buttonRect;  // To store button dimensions

        public Button(ContentManager content, Vector2 position)
        {
            _menuButtonTexture = content.Load<Texture2D>("menu_button01");
            _smollButtonTexture = content.Load<Texture2D>("Smoll_Button");
            _position = position;

            // Initialize the button rectangle for size detection
            _buttonRect = new Rectangle((int)_position.X, (int)_position.Y, _menuButtonTexture.Width, _menuButtonTexture.Height);
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw the appropriate button texture depending on whether it's clicked or not
            if (_isPressed)
            {
                sb.Draw(_smollButtonTexture, _position, _clickedColor);
            }
            else if (_isHovered)
            {
                sb.Draw(_smollButtonTexture, _position, _hoveredColor);
            }
            else
            {
                sb.Draw(_menuButtonTexture, _position, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        { // Get current mouse state
            MouseState mouseState = Mouse.GetState();

            // Check if the mouse is hovering over the button
            _isHovered = _buttonRect.Contains(mouseState.Position);

            // Handle mouse click
            if (_isHovered && mouseState.LeftButton == ButtonState.Pressed)
            {
                _isPressed = true;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                _isPressed = false;
            }
        }

        private bool isPressed()
        {
            return _isPressed;
        }

        private bool isHovered()
        {
            return _isHovered;
        }
    }
}
