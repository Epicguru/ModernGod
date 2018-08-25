using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World.Shrubs
{
    public class Grass : Shrub
    {
        public Grass()
            :base(1, "Grass")
        {
            SetTexture(2, 1, 1, 1);
            DefaultColour = Color.ForestGreen;
        }
    }
}
