using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Lethal_Organization
{
    interface ICommand<in T> where T : class
    {
        public bool Finished
        {
            set;
            get;
        }
        void Execute(T gameObject);

        void Update(T gameObject, GameTime gameTime);
    }
}
