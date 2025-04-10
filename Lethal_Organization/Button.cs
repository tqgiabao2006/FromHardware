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
    internal class Button
    {
        private Texture2D spriteSheet;
        private Rectangle normalSource, hoverSource, pressSource;
        private Rectangle textSource;
        private Vector2 position;
        private Rectangle bounds;
        private bool isHovered, isPressed;

        public Action OnClick;

        public Button(Texture2D spriteSheet, Rectangle normalSource, Rectangle hoverSource, Rectangle pressSource, Rectangle textSource, Vector2 position)
        {
            this.spriteSheet = spriteSheet;
            this.normalSource = normalSource;
            this.hoverSource = hoverSource;
            this.pressSource = pressSource;
            this.textSource = textSource;
            this.position = position;
            this.bounds = new Rectangle((int)position.X, (int)position.Y, normalSource.Width, normalSource.Height);
        }

        public void Update(MouseState currentMouse, MouseState previousMouse)
        {
            isHovered = bounds.Contains(currentMouse.Position);
            isPressed = isHovered && currentMouse.LeftButton == ButtonState.Pressed;

            if (isHovered && currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
            {
                OnClick?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle currentSource = normalSource;
            if (isPressed) currentSource = pressSource;
            else if (isHovered) currentSource = hoverSource;

            spriteBatch.Draw(spriteSheet, position, currentSource, Color.White);
            spriteBatch.Draw(spriteSheet, position, textSource, Color.White);
        }
    }
}
