using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lethal_Organization
{
    /// <summary>
    /// Platform bounds for each enemies movement
    /// </summary>
    struct SpawnInfo
    {
        //Index of tile in grid (gg sheet file) map design
        public int LeftIndex1; 
        public int LeftIndex2;

        public int RightIndex1;
        public int RightIndex2;
    }
}
