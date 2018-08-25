using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World.Shrubs
{
    public class Bush : Shrub
    {
        public Bush()
            :base(4, "Bush")
        {
            SetTexture(2, 2, 1, 1);
        }
    }
}
