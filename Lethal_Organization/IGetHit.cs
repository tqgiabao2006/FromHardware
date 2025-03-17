using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    internal interface IGetHit
    {
        public int HP { get; }
        public void GetHit(int damage);
    }
}
