using ModernGod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class MapGen
    {
        public Map Map { get; private set; }

        public MapGen(Map map)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "The map cannot be null!");
        }

        public void Generate()
        {
            // Make a road system...

            Queue<Vector2Int> open = new Queue<Vector2Int>();
            Dictionary<int, Vector2Int> closed = new Dictionary<int, Vector2Int>();

            Vector2Int center = new Vector2Int(Map.Width / 2, Map.Height / 2);
            open.Enqueue(center);
            open.Enqueue(center + new Vector2Int(1, 0));
            open.Enqueue(center + new Vector2Int(-1, 0));
            open.Enqueue(center + new Vector2Int(0, 1));
            open.Enqueue(center + new Vector2Int(0, -1));

            const int STOPPING_CHANCE = 40;
            const int BRANCH_CHANCE = 17;

            while (open.Count > 0)
            {
                var current = open.Dequeue();

                // Check surroundings...
                bool left = closed.ContainsKey(GetIndex(current.X - 1, current.Y));
                bool right = closed.ContainsKey(GetIndex(current.X + 1, current.Y));
                bool down = closed.ContainsKey(GetIndex(current.X, current.Y - 1));
                bool up = closed.ContainsKey(GetIndex(current.X, current.Y + 1));

                if(!left && !right && !down && !up)
                {
                    // Choose a random direction and expand to it!
                    bool horizontal = RandomState();
                    bool positive = RandomState();

                    int offX = horizontal ? (positive ? 1 : -1) : 0;
                    int offY = !horizontal ? (positive ? 1 : -1) : 0;

                    closed.Add(GetIndex(current.X, current.Y), current);
                    var newPos = current + new Vector2Int(offX, offY);

                    if (Map.InBounds(newPos.X, newPos.Y))
                        open.Enqueue(newPos);
                }
                else
                {
                    // Add to closed.
                    int index = GetIndex(current.X, current.Y);
                    if (!closed.ContainsKey(index))
                        closed.Add(index, current);
                    else
                        continue;

                    if (left)
                    {
                        // There is a path to the left, so naturally expand to the right.
                        // However, there should also be a stopping chance, and a chance to branch off to the top and bottom.

                        bool stop = rand.Next(STOPPING_CHANCE) == 0;
                        bool branch = rand.Next(BRANCH_CHANCE) == 0;

                        if (branch)
                        {
                            bool positive = RandomState();
                            var newPos = current + new Vector2Int(0, positive ? 1 : -1);
                            if (Map.InBounds(newPos.X, newPos.Y))
                                open.Enqueue(newPos);
                        }
                        if (!stop)
                        {
                            var newPos = current + new Vector2Int(1, 0);
                            if (Map.InBounds(newPos.X, newPos.Y) && !open.Contains(newPos))
                                open.Enqueue(newPos);
                        }
                    }
                    else if (right)
                    {
                        // There is a path to the right, so naturally expand to the left.
                        // However, there should also be a stopping chance, and a chance to branch off to the top and bottom.

                        bool stop = rand.Next(STOPPING_CHANCE) == 0;
                        bool branch = rand.Next(BRANCH_CHANCE) == 0;

                        if (branch)
                        {
                            bool positive = RandomState();
                            var newPos = current + new Vector2Int(0, positive ? 1 : -1);
                            if (Map.InBounds(newPos.X, newPos.Y))
                                open.Enqueue(newPos);
                        }
                        if (!stop)
                        {
                            var newPos = current + new Vector2Int(-1, 0);
                            if (Map.InBounds(newPos.X, newPos.Y) && !open.Contains(newPos))
                                open.Enqueue(newPos);
                        }
                    }
                    else if (down)
                    {
                        // There is a path below, so naturally expand upwards.
                        // However, there should also be a stopping chance, and a chance to branch off to the left and right.

                        bool stop = rand.Next(STOPPING_CHANCE) == 0;
                        bool branch = rand.Next(BRANCH_CHANCE) == 0;

                        if (branch)
                        {
                            bool positive = RandomState();
                            var newPos = current + new Vector2Int(positive ? 1 : -1, 0);
                            if (Map.InBounds(newPos.X, newPos.Y))
                                open.Enqueue(newPos);
                        }
                        if (!stop)
                        {
                            var newPos = current + new Vector2Int(0, 1);
                            if (Map.InBounds(newPos.X, newPos.Y) && !open.Contains(newPos))
                                open.Enqueue(newPos);
                        }
                    }
                    else if (up)
                    {
                        // There is a path above, so naturally expand downwards.
                        // However, there should also be a stopping chance, and a chance to branch off to the left and right.

                        bool stop = rand.Next(STOPPING_CHANCE) == 0;
                        bool branch = rand.Next(BRANCH_CHANCE) == 0;

                        if (branch)
                        {
                            bool positive = RandomState();
                            var newPos = current + new Vector2Int(positive ? 1 : -1, 0);
                            if (Map.InBounds(newPos.X, newPos.Y))
                                open.Enqueue(newPos);
                        }
                        if (!stop)
                        {
                            var newPos = current + new Vector2Int(0, -1);
                            if (Map.InBounds(newPos.X, newPos.Y) && !open.Contains(newPos))
                                open.Enqueue(newPos);
                        }
                    }
                }
            }

            foreach (var pair in closed)
            {
                Map.Terrain.SetTerrainTile(pair.Value.X, pair.Value.Y, TerrainTile.Dirt, false);
            }
        }

        private Random rand = new Random();
        private bool RandomState()
        {
            return rand.Next(2) == 0;
        }

        private int GetIndex(int x, int y)
        {
            if (!Map.InBounds(x, y))
                return -1;

            return x + y * Map.Width;
        }
    }
}
