using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal class Button: IDrawable, IUpdateable
    {
        private Rectangle _position;
        private Texture2D _texture;

        private Color _hoveredColor;
        private Color _clickedColor;

        public void Draw(SpriteBatch sb)
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        private bool isPressed()
        {
            return false;
        }

        private bool isHovered()
        {
            return false;
        }
    }
}
