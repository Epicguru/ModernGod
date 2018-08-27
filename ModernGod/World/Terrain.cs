using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Logging;
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

        public int SectorsX { get; private set; }
        public int SectorsY { get; private set; }
        public int SectorWidth { get; private set; }
        public int SectorHeight { get; private set; }
        public Map Map { get; private set; }
        public RenderTarget2D[,] RT { get; private set; }
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
        private List<KeyValuePair<TerrainRefreshType, Vector2Int>> refresh = new List<KeyValuePair<TerrainRefreshType, Vector2Int>>();

        public Terrain(Map map, int sectorsX, int sectorsY)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "The map for this terrain cannot be null!");

            if (sectorsX <= 0 || sectorsY <= 0)
                throw new ArgumentException("sectorsX or sectorsY", "The number of sectors may not be lower than 1!");

            this.SectorsX = sectorsX;
            this.SectorsY = sectorsY;

            if (map.Width % sectorsX != 0 || map.Height % sectorsY != 0)
                throw new ArgumentException("The width and height of the map must be perfectly divisible by the number of sectors, respectively!");
        }

        private RenderTarget2D CreateTarget(int width, int height)
        {
            return new RenderTarget2D(Main.Graphics.GraphicsDevice, width, height, false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }
        
        public void Init()
        {
            // Create the render target to draw all the tiles to, in order to display them quickly without having to re-draw.
            // TODO add the posibility of more than one render target to create huge maps.
            RT = new RenderTarget2D[SectorsX, SectorsY];
            int w = Map.Width / SectorsX;
            int h = Map.Height / SectorsY;
            for (int x = 0; x < SectorsX; x++)
            {
                for (int y = 0; y < SectorsY; y++)
                {
                    RT[x, y] = CreateTarget(w * TILE_SIZE, h * TILE_SIZE);
                    RT[x, y].Name = "Map '" + Map.Name + "' terrain texture (" + x + ", " + y + ")";
                }
            }
            this.SectorWidth = w;
            this.SectorHeight = h;

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
                    SetTerrainTile(x, y, TerrainTile.Grass, false);
                }
            }

            // Flag all as needing refresh so that they are drawn.
            for (int x = 0; x < SectorsX; x++)
            {
                for (int y = 0; y < SectorsY; y++)
                {
                    refresh.Add(new KeyValuePair<TerrainRefreshType, Vector2Int>(TerrainRefreshType.WHOLE_RT, new Vector2Int(x, y)));
                }
            }

            Logger.Log("Setup map terrain, split into {0}x{1} sectors, where each sector is {2}x{3} tiles.".Form(SectorsX, SectorsY, SectorWidth, SectorHeight));
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

        public void SetTerrainTile(int x, int y, TerrainTile tile, bool refresh = true)
        {
            if(Map.InBounds(x, y))
            {
                int index = GetIndex(x, y);
                SetTextureAt(index, tile.Textures[rand.Next(tile.Textures.Length)], refresh);
                SetColourAt(x, y, TryRegisterColour(tile.Colour), refresh);
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

        private void SetTextureAt(int index, byte id, bool refresh = true)
        {
            if(index >= 0 && index < types.Length)
            {
                if(types[index] != id)
                {
                    types[index] = id;
                    int x = index % Map.Width;
                    int y = index / Map.Width;
                    if(refresh)
                        PostRefresh(TerrainRefreshType.SINGLE_TILE, x, y);
                }
            }
        }

        private void PostRefresh(TerrainRefreshType type, int x, int y)
        {
            var request = new KeyValuePair<TerrainRefreshType, Vector2Int>(type, new Vector2Int(x, y));
            if (!refresh.Contains(request))
            {
                refresh.Add(request);
            }
        }

        private void SetColourAt(int x, int y, Color colour)
        {
            SetColourAt(x, y, TryRegisterColour(colour));
        }

        private void SetColourAt(int x, int y, byte id, bool refresh = true)
        {
            if(Map.InBounds(x, y))
            {
                int index = GetIndex(x, y);

                if(colours[index] != id)
                {
                    colours[index] = id;
                    if(refresh)
                        PostRefresh(TerrainRefreshType.SINGLE_TILE, x, y);
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

        public void RedrawRT(SpriteBatch spr, int tx, int ty)
        {

            Main.Graphics.GraphicsDevice.SetRenderTarget(this.RT[tx, ty]);
            Main.Graphics.GraphicsDevice.Clear(Color.Black);
            spr.Begin();
            int sx = tx * SectorWidth;
            int sy = ty * SectorHeight;
            int lx = 0, ly = 0;
            for (int x = sx; x < sx + SectorWidth; x++)
            {
                for (int y = sy; y < sy + SectorHeight; y++)
                {
                    Rectangle pos = new Rectangle(lx * TILE_SIZE, ly * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    Rectangle source = TerrainAtlas.GetSprite(GetTypeAt(x, y));
                    spr.Draw(TerrainAtlas.Texture, pos, source, GetColourAt(x, y));
                    ly++;
                }
                lx++;
                ly = 0;
            }
            spr.End();
            Main.Graphics.GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawToRT(SpriteBatch spr, int x, int y)
        {
            Rectangle pos = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            Rectangle source = TerrainAtlas.GetSprite(GetTypeAt(x, y));
            spr.Draw(TerrainAtlas.Texture, pos, source, GetColourAt(x, y));
        }

        public void TargetDraw(SpriteBatch spr)
        {
            foreach (var pair in refresh)
            {
                if(pair.Key == TerrainRefreshType.WHOLE_RT)
                {
                    RedrawRT(spr, pair.Value.X, pair.Value.Y);
                }
            }

            bool drawing = false;

            int oldX = -1, oldY = -1;
            foreach (var pair in refresh)
            {
                if (pair.Key == TerrainRefreshType.SINGLE_TILE)
                {
                    var pos = pair.Value;
                    int rx = pos.X / SectorWidth;
                    int ry = pos.Y / SectorHeight;
                    int lx = pos.X % SectorWidth;
                    int ly = pos.Y % SectorHeight;

                    if(oldX != rx || oldY != ry)
                    {
                        oldX = rx;
                        oldY = ry;

                        if (drawing)
                        {
                            spr.End();
                            drawing = false;
                        }
                        Main.Graphics.GraphicsDevice.SetRenderTarget(this.RT[rx, ry]);
                        spr.Begin();
                        drawing = true;
                    }

                    DrawToRT(spr, lx, ly);
                }
            }

            if (drawing)
                spr.End();

            foreach (var pair in refresh)
            {
                if (pair.Key == TerrainRefreshType.WHOLE_RT)
                {
                    var pos = pair.Value;
                    int rx = pos.X;
                    int ry = pos.Y;

                    RedrawRT(spr, rx, ry);
                }
            }

            refresh.Clear();
        }

        public void Draw(SpriteBatch spr)
        {
            for (int x = 0; x < SectorsX; x++)
            {
                for (int y = 0; y < SectorsY; y++)
                {
                    var rt = RT[x, y];
                    Rectangle dest = new Rectangle(x * SectorWidth * TILE_SIZE, y * SectorHeight * TILE_SIZE, SectorWidth * TILE_SIZE, SectorHeight * TILE_SIZE);
                    spr.Draw(rt, dest, rt.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in RT)
            {
                item.Dispose();
            }
            RT = null;

            TerrainAtlas.Dispose();
        }
    }

    public enum TerrainRefreshType : byte
    {
        SINGLE_TILE,
        WHOLE_RT
    }
}
