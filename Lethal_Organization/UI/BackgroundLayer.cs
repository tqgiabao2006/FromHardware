using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Lethal_Organization
{
    public struct BackgroundLayer
    {
        [Range(0,1)]
        public float Speed;
        public Texture2D Texture;
    }
}
