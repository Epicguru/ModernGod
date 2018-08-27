using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Logging;
using ModernGod.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class BuildingManager : IDisposable
    {
        public Map Map { get; private set; }
        public Texture2D BuildingsTexture;
        
        private SortedSet<int> filledIndices = new SortedSet<int>();
        private List<Building> buildings = new List<Building>();

        public BuildingManager(Map map)
        {
            Map = map ?? throw new ArgumentNullException("map", "Map cannot be null!");
        }

        public void Init()
        {
            BuildingsTexture = Main.ContentMangager.Load<Texture2D>("Textures/Buildings");
            Logger.Log(BuildingsTexture);

            Random r = new Random();
            int cx = Map.Width / 2;
            int cy = Map.Height / 2;

            for (int i = 0; i < 20; i++)
            {
                Building b = new Building(this, new Rectangle(r.Next(cx - 40, cx + 41), r.Next(cy - 40, cy + 41), r.Next(3, 7), r.Next(3, 7)));
                RegisterBuilding(b);
            }
        }

        public void RegisterBuilding(Building b)
        {
            if (b == null || buildings.Contains(b))
                return;

            buildings.Add(b);

            for (int x = 0; x < b.Bounds.Width; x++)
            {
                for (int y = 0; y < b.Bounds.Height; y++)
                {
                    int X = b.Bounds.X + x;
                    int Y = b.Bounds.Y + y;
                    int index = X + Y * Map.Width;

                    int internalIndex = x + y * b.Bounds.Width;

                    if (b.Filled[internalIndex])
                    {
                        bool good = filledIndices.Add(index);
                    }                    
                }
            }
        }

        public bool TileHasBuilding(int x, int y)
        {
            int index = x + y * Map.Width;
            return filledIndices.Contains(index);
        }

        public void Update()
        {
           
        }

        public void Draw(SpriteBatch spr)
        {
            foreach (var building in buildings)
            {
                if (building == null)
                    continue;

                building.Draw(spr);
            }
        }

        public void Dispose()
        {
            BuildingsTexture.Dispose();
            BuildingsTexture = null;
        }
    }
}
