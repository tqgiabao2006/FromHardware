using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal interface IStateChange
    {
        public void OnStateChange(GameManager.GameState state);
    }
}
