using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Textures;
using ModernGod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class Terrain : IDisposable
    {
        // Stores info about a map's terrain, such as ground type. 
        // Also stores ground colour and similar stats.

        public TextureAtlas<byte> TerrainAtlas { get; private set; }
        public const int ATLAS_WIDTH = 4;
        public const int ATLAS_HEIGHT = 1;

        public const int TILE_SIZE = 16;

        public Map Map { get; private set; }
        public RenderTarget2D RT { get; private set; }
        public bool Dirty { get; private set; } = false;
        public int ArrayMemoryUsage
        {
            get
            {
                return types.Length * 1 + colours.Length * 1;
            }
        }

        private Dictionary<byte, Color> IDToColour;
        private Dictionary<Color, byte> colourToID;
        private byte highestID = 0;
        private byte[] types;
        private byte[] colours;
        private Random rand = new Random();

        public Terrain(Map map)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "The map for this terrain cannot be null!");                     
        }

        public void Init()
        {
            // Create the render target to draw all the tiles to, in order to display them quickly without having to re-draw.
            // TODO add the posibility of more than one render target to create huge maps.
            RT = new RenderTarget2D(Main.Graphics.GraphicsDevice, Map.Width * TILE_SIZE, Map.Height * TILE_SIZE, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            RT.Name = "Map '{0}' terrain texture".Form(Map.Name);

            // Loads the textures.
            LoadTextures();

            // Create data structures.
            CreateArrays();

            var noise = GenerateNoise(Map.Width, Map.Height, 0, 0, 1f);
            Random r = new Random();
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    //SetTerrainTile(x, y, noise[x, y] < 0.4f ? TerrainTile.Dirt : TerrainTile.Grass);
                    SetTerrainTile(x, y, TerrainTile.Grass);
                }
            }

            // Flag as dirty so that it is re-drawn.
            Dirty = true;
        }

        private float[,] GenerateNoise(int width, int height, float x, float y, float scale)
        {
            float[,] map = new float[width, height];
            const float GENERAL_MAGNITUDE = 100f;

            for (int X = 0; X < width; X++)
            {
                for (int Y = 0; Y < height; Y++)
                {
                    map[X, Y] = Perlin.Remap(Perlin.Noise((X + x) / GENERAL_MAGNITUDE * scale, (Y + y) / GENERAL_MAGNITUDE * scale));
                }
            }

            return map;
        }

        public void SetTerrainTile(int x, int y, TerrainTile tile)
        {
            if(Map.InBounds(x, y))
            {
                int index = GetIndex(x, y);
                SetTextureAt(index, tile.Textures[rand.Next(tile.Textures.Length)]);
                SetColourAt(x, y, TryRegisterColour(tile.Colour));
            }
        }

        public byte GetTypeAt(int x, int y)
        {
            if(Map.InBounds(x, y))
            {
                return types[GetIndex(x, y)];
            }
            else
            {
                return 0;
            }
        }

        public byte GetColourIDAt(int x, int y)
        {
            if (Map.InBounds(x, y))
            {
                return colours[GetIndex(x, y)];
            }
            else
            {
                return 0;
            }
        }

        private void CreateArrays()
        {
            types = new byte[Map.Area];
            colours = new byte[Map.Area];

            highestID = 0;
            colourToID = new Dictionary<Color, byte>();
            IDToColour = new Dictionary<byte, Color>();

            TryRegisterColour(Color.White);
        }

        private void SetTextureAt(int index, byte id)
        {
            if(index >= 0 && index < types.Length)
            {
                if(types[index] != id)
                {
                    types[index] = id;
                    Dirty = true;
                }
            }
        }

        private void SetColourAt(int x, int y, Color colour)
        {
            SetColourAt(x, y, TryRegisterColour(colour));
        }

        private void SetColourAt(int x, int y, byte id)
        {
            if(Map.InBounds(x, y))
            {
                int index = GetIndex(x, y);

                if(colours[index] != id)
                {
                    colours[index] = id;
                    Dirty = true;
                }
            }
        }

        private Color GetColourAt(int x, int y)
        {
            if(Map.InBounds(x, y))
            {
                return this.IDToColour[colours[GetIndex(x, y)]];
            }
            else
            {
                return Color.White;
            }
        }

        private Color GetColourFromID(byte id)
        {
            return IDToColour[id];
        }

        private byte GetColourID(Color colour)
        {
            return colourToID[colour];
        }

        private byte TryRegisterColour(Color colour)
        {
            // Tries to register a colour. If the colour is already registered, the existing ID is returned.
            // If it is not registered, it is added and the new ID is returned.

            if (!colourToID.ContainsKey(colour))
            {
                if(highestID == 255)
                {
                    throw new Exception("Ran out of memory to address colours, consider upgrading from a single byte!");
                }
                byte id = highestID++;
                colourToID.Add(colour, id);
                IDToColour.Add(id, colour);

                return id;
            }
            else
            {
                return colourToID[colour];
            }            
        }

        private int GetIndex(int x, int y)
        {
            return x + y * Map.Width;
        }

        private void LoadTextures()
        {
            // Load the terrain atlas.
            if (TerrainAtlas == null)
            {
                var tex = Main.ContentMangager.Load<Texture2D>("Textures/Default Terrain");
                TerrainAtlas = new TextureAtlas<byte>(tex);
                byte[] ids = new byte[4];
                for (byte i = 0; i < ATLAS_WIDTH * ATLAS_HEIGHT; i++)
                {
                    ids[i] = i;
                }
                TerrainAtlas.AddGridSprites(ids, ATLAS_WIDTH, ATLAS_HEIGHT, TILE_SIZE, TILE_SIZE);
            }
        }

        public void TargetDraw(SpriteBatch spr)
        {
            if (!Dirty)
                return;
            Dirty = false;

            Main.Graphics.GraphicsDevice.SetRenderTarget(this.RT);
            Main.Graphics.GraphicsDevice.Clear(Color.Black);
            spr.Begin();
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    Rectangle pos = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    Rectangle source = TerrainAtlas.GetSprite(GetTypeAt(x, y));
                    spr.Draw(TerrainAtlas.Texture, pos, source, GetColourAt(x, y));
                }
            }
            spr.End();
            Main.Graphics.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spr)
        {          
            spr.Draw(RT, Vector2.Zero, Color.White);
        }

        public void Dispose()
        {
            RT.Dispose();
            RT = null;

            TerrainAtlas.Dispose();
        }
    }
}
