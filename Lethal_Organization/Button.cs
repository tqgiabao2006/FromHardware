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
        private float _scale;

        private Color _defaultColor = Color.White;
        private Color _hoveredColor = Color.Gray;
        private Color _clickedColor = Color.Black;

        private bool _isHovered;
        private bool _isPressed;

        private Rectangle _buttonRect;  // To store button dimensions
        private Rectangle _sourceRectangle;

        public Button(ContentManager content, Vector2 position)
        {
            _menuButtonTexture = content.Load<Texture2D>("menu_button01");
            _smollButtonTexture = content.Load<Texture2D>("Smoll_Button");
            _position = position;
            _scale = 3f;
            _sourceRectangle = new Rectangle ((int)_position.X, (int)_position.Y, _menuButtonTexture.Width, _menuButtonTexture.Height);
          
            // Initialize the button rectangle for size detection
            _buttonRect = new Rectangle((int)_position.X, (int)_position.Y, _menuButtonTexture.Width*2, _menuButtonTexture.Height*3);
        }

        public void Draw(SpriteBatch sb, bool isDebug)
        {
            if (isDebug)
            {
                CustomDebug.DrawWireRectangle(sb, _buttonRect, 0.5f, Color.Aqua);
            }
            // Draw the appropriate button texture depending on whether it's clicked or not
            if (_isPressed)
            {
                sb.Draw(_menuButtonTexture, _position, _sourceRectangle, _clickedColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
            else if (_isHovered)
            {
                sb.Draw(_menuButtonTexture, _position, _sourceRectangle, _hoveredColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
            else
            {
                sb.Draw(_menuButtonTexture, _position, _sourceRectangle, _defaultColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
        }

        public void Update(GameTime gameTime)
        {   
            // Get current mouse state
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
