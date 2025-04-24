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

namespace Lethal_Organization.UI
{
    internal class Button: IUpdateable
    {
        private Color _hoverColor;
        
        private float _hoverScale;
        
        private Texture2D _texture;

        private Rectangle _displayPos;

        private Action _toggleOn;

        private Action _toggleOff;

        private bool _isToggled;

        private MouseState _prevMouseState;

        private MouseState _currMouseState;

        private bool _isHovered;

        private bool _isPressed;

        private AudioManager _audioManager;
        public Button(AudioManager audioManager, Texture2D texture, Rectangle displayPos, Action toggleOn, Action toggleOff = null)
        {
            _audioManager = audioManager;

            _displayPos = displayPos;
            
            _texture = texture; 
            
            _toggleOn = toggleOn;

            _toggleOff = toggleOff;

            _isToggled = false;

            _hoverColor = Color.OrangeRed;

            _hoverScale = 1.1f;

        }

        public void Draw(SpriteBatch sb)
        {
            if (_isHovered)
            {
                sb.Draw(_texture, new Vector2(_displayPos.X, _displayPos.Y), null, _hoverColor, 0f, Vector2.Zero, _hoverScale, SpriteEffects.None, 0f);                   
            }
            else
            {
                sb.Draw(_texture, new Vector2(_displayPos.X, _displayPos.Y), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            }
        }

        public void Draw(SpriteBatch sb, Rectangle press, Rectangle hover, Rectangle normal)
        {
            if(_isHovered)
            {
                sb.Draw(_texture, _displayPos, hover, _hoverColor, 0f, Vector2.Zero,  SpriteEffects.None, 0f);
            }
            else if(_isPressed)
            {
                sb.Draw(_texture, _displayPos, press, _hoverColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
            {
                sb.Draw(_texture, _displayPos, normal, _hoverColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
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

                if(_toggleOn != null && !_isToggled)
                {
                    _toggleOn();
                    _isToggled = true;
                    _audioManager.PlaySFX(AudioManager.SFXID.ButtonClick);
                }else if(_isToggled)
                {
                    if(_toggleOff != null)
                    {
                        _toggleOff();   
                    }else if(_toggleOn != null)
                    {
                        _toggleOn();
                    }
                    _audioManager.PlaySFX(AudioManager.SFXID.ButtonClick);
                    _isToggled = false;
                }
            }
            
            _prevMouseState = _currMouseState;
        }
    }
}
