using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Debugging;
using ModernGod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Textures
{
    public class TextureAtlas<T> : IDisposable
    {
        public Texture2D Texture { get; private set; }

        private Dictionary<T, Rectangle> map = new Dictionary<T, Rectangle>();

        public TextureAtlas(Texture2D texture)
        {
            this.Texture = texture;
        }

        public bool AddGridSprites(T[] ids, int columns, int rows, int width, int height)
        {
            // Index of names is:
            // index = x + y * width;
            // Therefore:
            // x = index % width
            // y = index / width

            if (columns == 0 || rows == 0)
            {
                Debug.LogError("Did not add any grid sprites to this atlas, row or column count was 0!");
                return false;
            }

            if(ids == null || ids.Length == 0)
            {
                Debug.LogError("Names array is null or empty, no grid sprites were added!");
                return false;
            }

            if(ids.Length != rows * columns)
            {
                Debug.LogError("Incorrect number of names to add grid sprites, there are {0} supplied names but there should be {1}".Form(ids.Length, rows * columns));
                return false;
            }

            bool allworked = true;
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int X = x * width;
                    int Y = y * height;
                    Rectangle r = new Rectangle(X, Y, width, height);
                    int index = x + y * width;
                    T id = ids[index];

                    bool worked = AddSprite(id, r);
                    if (!worked)
                        allworked = false;
                }
            }

            return allworked;
        }

        public bool AddSprite(T id, Rectangle bounds)
        {
            if (map.ContainsKey(id))
            {
                Debug.LogError("Cannot add '{0}' to this texture atlas, there is already a sprite for that ID!".Form(id));
                return false;
            }

            map.Add(id, bounds);

            return true;
        }

        public Rectangle GetSprite(T id)
        {
            if (!map.ContainsKey(id))
                return Rectangle.Empty;

            return map[id];
        }

        public void Dispose()
        {
            Texture.Dispose();
            Texture = null;

            map.Clear();
            map = null;
        }
    }
}
