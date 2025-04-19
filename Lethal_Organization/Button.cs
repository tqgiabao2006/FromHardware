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
    internal class Button: IUpdateable
    {
        private Color _hoverColor;
        
        private float _hoverScale;
        
        private Texture2D _texture;

        private Rectangle _displayPos;

        private Action _onClick;

        private MouseState _prevMouseState;

        private MouseState _currMouseState;

        private bool _isHovered;

        public Button(Texture2D texture, Rectangle displayPos, Action onClick)
        {
            _displayPos = displayPos;
            
            _texture = texture; 
            
            _onClick = onClick;

            _hoverColor = Color.OrangeRed;

            _hoverScale = 1.2f;

        }

        public void Draw(SpriteBatch sb)
        {
            if (_isHovered)
            {
                sb.Draw(_texture, new Vector2(_displayPos.X, _displayPos.Y), null, _hoverColor, 0f, new Vector2(0.5f, 0.5f), _hoverScale, SpriteEffects.None, 0f);                   
            }
            else
            {
                sb.Draw(_texture, new Vector2(_displayPos.X, _displayPos.Y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            }
        }

        public void Update(GameTime gameTime)
        {
            _currMouseState = Mouse.GetState();

            if (_displayPos.Contains(_currMouseState.Position))
            {
                _isHovered = true;
            }
            else
            {
                _isHovered = false;
            }

            if (_isHovered
                && _prevMouseState.LeftButton == ButtonState.Pressed
                && _currMouseState.LeftButton == ButtonState.Released)
            {

               _onClick?.Invoke();
            }
            
            _prevMouseState = _currMouseState;
        }
    }
}
