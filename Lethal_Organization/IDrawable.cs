using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal interface IDrawable
    {
        public void Draw(SpriteBatch sb, bool isDebug);
    }
}
