using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public struct TerrainTile
    {
        public static TerrainTile Dirt = new TerrainTile("Dirt", Color.SandyBrown, 0, 1, 2, 3);
        public static TerrainTile Grass = new TerrainTile("Grass", Color.LawnGreen, 0, 1, 2, 3);

        public byte[] Textures;
        public Color Colour;
        public string Name;

        public TerrainTile(string name, Color colour, params byte[] textures)
        {
            if (textures.Length == 0)
                throw new ArgumentException("Texture index array length cannot be 0!");

            Textures = textures;
            Colour = colour;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
