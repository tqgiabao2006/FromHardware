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
        private Texture2D _buttonTexture;
        private Vector2 _position;
        private Rectangle _bounds;
        private float _scale;

        private Color _defaultColor = Color.White;
        private Color _hoveredColor = Color.Gray;
        private Color _clickedColor = Color.Black;

        private bool _isHovered;
        private bool _isPressed;

        private Rectangle _worldPos;  // To store button dimensions
        private Rectangle _sourceImg;

        public Button(Texture2D buttonTexture, Vector2 position)
        {
            _buttonTexture = buttonTexture;
            _position = position;
            _scale = 3f;

            _sourceImg = new Rectangle ((int)_position.X, (int)_position.Y, _buttonTexture.Width, _buttonTexture.Height);
          
            // Initialize the button rectangle for size detection
            _worldPos = new Rectangle((int)_position.X, (int)_position.Y, _buttonTexture.Width*2, _buttonTexture.Height*3);
        }

        public void Draw(SpriteBatch sb, bool isDebug)
        {
            if (isDebug)
            {
                CustomDebug.DrawWireRectangle(sb, _worldPos, 0.5f, Color.Aqua);
            }

            // Draw the appropriate button texture depending on whether it's clicked or not
            if (_isPressed)
            {
                sb.Draw(_buttonTexture, _position, _sourceImg, _clickedColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
            else if (_isHovered)
            {
                sb.Draw(_buttonTexture, _position, _sourceImg, _hoveredColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
            else
            {
                sb.Draw(_buttonTexture, _position, _sourceImg, _defaultColor, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Get current mouse state
            MouseState mouseState = Mouse.GetState();

            // Check if the mouse is hovering over the button
            _isHovered = _worldPos.Contains(mouseState.Position);

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
    }
}
