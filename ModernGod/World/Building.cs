using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class Building
    {
        public BuildingManager Manager { get; private set; }
        public BuildingType Type = BuildingType.FLATS;

        private const int TILE_SIZE = 16;
        private static Rectangle[] Sources
        {
            get
            {
                if(_sources == null)
                {
                    _sources = new Rectangle[16];

                    for (int i = 0; i < 16; i++)
                    {
                        int index = sourcesIndex[i];
                        int x = index % 3;
                        int y = index / 3;

                        _sources[i] = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    }
                }

                return _sources;
            }
        }
        private static Rectangle[] _sources;
        private static byte[] sourcesIndex = new byte[]
        {
            /*0000*/ 4,
            /*0001*/ 7,
            /*0010*/ 3,
            /*0011*/ 6,
            /*0100*/ 1,
            /*0101*/ 4,
            /*0110*/ 0,
            /*0111*/ 3,
            /*1000*/ 5,
            /*1001*/ 8,
            /*1010*/ 4,
            /*1011*/ 7,
            /*1100*/ 2,
            /*1101*/ 5,
            /*1110*/ 1,
            /*1111*/ 4
        };

        public string Name;
        public Rectangle Bounds { get; private set; }
        public bool[] Filled;

        public Building(BuildingManager manager, Rectangle bounds)
        {
            Manager = manager ?? throw new ArgumentNullException("manager", "Building manager cannot be null!");
            SetBounds(bounds);
        }

        private void SetBounds(Rectangle b)
        {
            Bounds = b;
            Filled = new bool[b.Width * b.Height];
            for (int i = 0; i < Filled.Length; i++)
            {
                Filled[i] = true;
            }
            Bounds = b;
        }

        public bool PointSolid(int globalX, int globalY)
        {
            if (!InBounds(globalX, globalY))
                return false;

            int lx = globalX - Bounds.X;
            int ly = globalY - Bounds.Y;

            int index = GetIndex(lx, ly);

            return Filled[index];
        }

        public bool InLocalBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Bounds.Width && y < Bounds.Height;
        }

        public bool InBounds(int x, int y)
        {
            return x >= Bounds.X && y >= Bounds.Y && x < Bounds.X + Bounds.Width && y < Bounds.Y + Bounds.Height;
        }

        private int GetIndex(int x, int y)
        {
            return x + y * Bounds.Width;
        }

        private int GetSurrounding(int x, int y)
        {
            int i = 0;
            if (InLocalBounds(x, y))
            {
                int index = GetIndex(x, y);
                bool filled = Filled[index];
                if (filled)
                    i++;
            }

            return i;
        }

        private int GetDrawIndex(int lx, int ly)
        {
            // Left.
            int left = GetSurrounding(lx - 1, ly);

            // Right.
            int right = GetSurrounding(lx + 1, ly);

            // Down.
            int down = GetSurrounding(lx, ly - 1);

            // Up.
            int up = GetSurrounding(lx, ly + 1);

            int index = left * 8 + up * 4 + right * 2 + down;

            return index;
        }

        private Rectangle GetSource(int lx, int ly, byte texture)
        {
            int index = GetDrawIndex(lx, ly);
            var rect = Sources[index];

            const byte WIDTH = 5;
            int x = texture % WIDTH;
            int y = texture / WIDTH;

            return new Rectangle(rect.X + x * TILE_SIZE * 3, rect.Y + y * TILE_SIZE * 3, rect.Width, rect.Height); ;
        }

        public void Draw(SpriteBatch spr)
        {
            int lx = 0;
            int ly = 0;
            for (int x = Bounds.X; x < Bounds.X + Bounds.Width; x++)
            {
                ly = 0;
                for (int y = Bounds.Y; y < Bounds.Y + Bounds.Height; y++)
                {
                    // TODO work out which one to draw

                    int index = GetIndex(lx, ly);
                    if (!Filled[index])
                    {
                        ly++;
                        continue;
                    }

                    byte tex = (byte)Type;
                    var source = GetSource(lx, ly, tex);
                    var dest = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);

                    spr.Draw(Manager.BuildingsTexture, dest, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Manager.Map.GetDepth(dest, DepthBias.BUILDING));
                    ly++;
                }
                lx++;
            }
        }
    }
}
