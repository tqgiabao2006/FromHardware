using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public Button(Texture2D menuButtonTexture, Texture2D smollButtonTexture, Vector2 position)
        {
            _menuButtonTexture = menuButtonTexture;
            _smollButtonTexture = smollButtonTexture;
            _position = position;

            _bounds = new Rectangle((int)position.X, (int)position.Y, menuButtonTexture.Width, menuButtonTexture.Height);
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D textureToDraw;
            Color colorToUse;

            if (_isHovered)
            {
                textureToDraw = _smollButtonTexture;
                colorToUse = _hoveredColor;
            }
            else
            {
                textureToDraw = _menuButtonTexture;
                colorToUse = _defaultColor;
            }

            if (_isPressed)
            {
                colorToUse = _clickedColor;
            }

            sb.Draw(textureToDraw, _position, colorToUse);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            _isHovered = _bounds.Contains(mouse.Position);
            _isPressed = _isHovered && mouse.LeftButton == ButtonState.Pressed;
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
