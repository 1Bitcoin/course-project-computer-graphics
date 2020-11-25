using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG
{
    class AllParameters
    {
        public int x, y, endx, endy;
        public int workFlag;

        public AllParameters(int x, int y, int endx, int endy, ref int workFlag)
        {
            this.x = x;
            this.y = y;
            this.endx = endx;
            this.endy = endy;
            this.workFlag = workFlag;
        }
    }
}
