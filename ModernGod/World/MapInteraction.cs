using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class MapInteraction
    {
        // Manages queries and interaction with a map, including checking map positions.

        public Map Map { get; private set; }

        public MapInteraction(Map map)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "The value of Map cannot be null!");
        }

        public bool IsTileWalkable_Unsafe(int x, int y)
        {
            // Check buildings...
            if (Map.Buildings.TileHasBuilding(x, y))
                return false;

            return true;
        }

        public bool IsTileWalkable(int x, int y)
        {
            if(Map.InBounds(x, y))
            {
                return IsTileWalkable_Unsafe(x, y);
            }
            else
            {
                return false;
            }
        }
    }
}
