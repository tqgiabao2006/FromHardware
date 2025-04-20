using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    public interface IGetHit
    {
        public int CurHP { get; }
        public void GetHit(int damage);
    }
}
