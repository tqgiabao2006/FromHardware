using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    interface UI
    {
        public bool Visible { get; set; }
        public void Hide();

        public void Show();

        public void Draw(SpriteBatch sb);

        public void Update(GameTime gameTime);
    }
}
