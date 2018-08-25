using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Characters;
using ModernGod.Characters.Pathfinding;
using ModernGod.Pathfinding;
using ModernGod.Utils;
using ModernGod.World.Shrubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public class Map : IDisposable
    {
        // Represents an area of the world that is in constant simulation, and all loaded in memory.

        public string Name { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float DepthBiasValue { get; private set; }
        public int Area
        {
            get
            {
                return Width * Height;
            }
        }

        public bool Initialized { get; private set; }

        public Terrain Terrain;
        public BuildingManager Buildings;
        public MapGen Generation;
        public CharacterManager Characters;
        public MapInteraction Interaction;
        public PathManager Pathing;
        public MapShrubs Shrubs;

        public Map(string name, int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException("Width must be at least 1!");

            if (height <= 0)
                throw new ArgumentException("Height must be at least 1!");

            Width = width;
            Height = height;
            Name = name;
            float tileDepthChange = 1f / Height;
            DepthBiasValue = -tileDepthChange / 20f;
        }

        public float GetDepth(Rectangle destination, int bias = 0)
        {
            return GetDepth(destination.Y, bias);
        }

        public float GetDepth(int y, int bias = 0)
        {
            return GetDepth((float)y, bias);
        }

        public float GetDepth(float y, int bias = 0)
        {
            float bottom = Height * 16f;
            return MathUtils.Clamp01((bottom - y) / bottom) + DepthBiasValue * bias;
        }

        Stopwatch watch = new Stopwatch();
        public void Init()
        {
            if (Initialized)
                return;

            Terrain = new Terrain(this);
            Buildings = new BuildingManager(this);
            Generation = new MapGen(this);
            Characters = new CharacterManager(this);
            Interaction = new MapInteraction(this);
            Pathing = new PathManager(4, this);
            Shrubs = new MapShrubs(this);

            Terrain.Init();
            Buildings.Init();
            Shrubs.Init();
            Generation.Generate();
            Pathing.Start();
            Characters.Init();

            Initialized = true;

            watch.Start();
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 100; i++)
                {
                    Characters.CreateCharacter(new Vector2Int(i), "John");
                }
            }                     
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public override string ToString()
        {
            return "{0} ({1} x {2})".Form(Name, Width, Height);
        }

        public void Update()
        {
            if (!Initialized)
                return;

            Buildings.Update();
            Characters.Update();

            if(Character.Completed == 500 && watch.IsRunning)
            {
                watch.Stop();
                Debugging.Debug.Log("Took " + watch.Elapsed.TotalSeconds + " for 500 paths.", ConsoleColor.Green);
            }
        }

        public void TargetDraw(SpriteBatch spr)
        {
            // Used to draw stuff to custom targets, instead of into the game world.
            if (!Initialized)
                return;

            Terrain.TargetDraw(spr);
        }

        public void Draw(SpriteBatch spr)
        {
            if (!Initialized)
                return;

            Terrain.Draw(spr);
            Characters.Draw(spr);
            Buildings.Draw(spr);
            Shrubs.Draw(spr);
        }

        public void Dispose()
        {
            if (!Initialized)
                return;

            Terrain.Dispose();
            Buildings.Dispose();
            Pathing.Dispose();
            Shrubs.Dispose();
        }
    }
}
