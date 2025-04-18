using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    struct Skill<T>
    {
        public T State;
        public int Rate; //For random chosen between skill
        public int Damage;
    }
}
