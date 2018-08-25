using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World.Shrubs
{
    public class TallTree : Shrub
    {
        public TallTree()
            :base(3, "Tall Tree")
        {
            SetTexture(0, 0, 1, 3);
        }
    }
}
