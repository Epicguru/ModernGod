using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World.Shrubs
{
    public class Tree : Shrub
    {
        public Tree()
            : base(2, "Tree")
        {
            SetTexture(1, 1, 1, 2);
        }
    }
}
