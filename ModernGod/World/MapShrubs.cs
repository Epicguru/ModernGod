using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class MapShrubs : IDisposable
    {
        // Map shrubbery objects, such as grass and trees.

        public Map Map { get; private set; }
        public Texture2D ShrubTexture { get; set; }

        private byte[] IDs;

        public MapShrubs(Map map)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "Value of the map object cannot be null!");
        }

        public void Draw(SpriteBatch spr)
        {
            int width = Map.Width;
            int height = Map.Height;
            byte lastID = 0;
            Shrub s = null;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = x + y * width;
                    byte id = IDs[index];

                    if(id != 0)
                    {
                        if(lastID != id)
                        {
                            lastID = id;
                            s = Shrub.Get(id);
                        }

                        if(s != null)
                        {
                            var colour = s.GetColour(x, y, Map);
                            var source = s.TextureBounds;
                            var dest = new Rectangle(x * 16, y * 16 - (source.Height - 16), source.Width, source.Height);

                            spr.Draw(ShrubTexture, dest, source, colour, 0f, Vector2.Zero, SpriteEffects.None, Map.GetDepth(y * 16, DepthBias.SHRUBBERY));
                        }
                    }
                }
            }
        }

        public void Init()
        {
            ShrubTexture = Main.ContentMangager.Load<Texture2D>("Textures/Shrubbery");
            Shrub.LoadShrubs();

            IDs = new byte[Map.Area];

            Random r = new Random();
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if(r.Next(0, 8) == 0)
                    {
                        SetShrub(x, y, (byte)r.Next(0, Shrub.HighestID + 1));
                    }
                }
            }
        }

        public void SetShrub(int x, int y, Shrub shrub)
        {
            if(shrub == null)
            {
                SetShrub(x, y, 0);
            }
            else
            {
                SetShrub(x, y, shrub.ID);
            }
        }

        public void SetShrub(int x, int y, byte ID)
        {
            if (!Map.InBounds(x, y))
                return;

            int index = GetIndex(x, y);
            IDs[index] = ID;
        }

        public byte GetShrubID(int x, int y)
        {
            if (!Map.InBounds(x, y))
                return 0;

            return IDs[GetIndex(x, y)];
        }

        public Shrub GetShrub(int x, int y)
        {
            return Shrub.Get(GetShrubID(x, y));
        }

        private int GetIndex(int x, int y)
        {
            return x + y * Map.Width;
        }

        public void Dispose()
        {
            Shrub.UnloadShrubs();
            ShrubTexture.Dispose();
        }
    }
}
