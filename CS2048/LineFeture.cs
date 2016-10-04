using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class LineFeature : Feature
    {
        public int index;

        public LineFeature(int index)
        {
            tuples = new float[65536];
            this.index = index;
        }

        public override int GetIndex(ulong blocks)
        {
            return Board.GetColumn(blocks, 3 - index);
        }
    }
}
