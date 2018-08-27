using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Utils;
using ModernGod.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModernGod.Logging
{
    public static class Logger
    {
        public const int FPS_COUNTER_SMOOTHING = 10;
        private static Queue<float> ups = new Queue<float>();
        private static Queue<float> fps = new Queue<float>();

        public static void Log(object o, ConsoleColor? colour = null)
        {
            if(colour == null)
            {
                Console.WriteLine(o == null ? "(null)" : o.ToString());
            }
            else
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = (ConsoleColor)colour;
                Console.WriteLine(o == null ? "(null)" : o.ToString());
                Console.ForegroundColor = old;

            }
        }

        public static void LogError(object o)
        {
            Log("[ERROR] " + (o == null ? "(null)" : o.ToString()), ConsoleColor.Red);
        }

        public static void DrawStats(SpriteBatch spr, GameTime time)
        {
            float cups = 1f / Time.UnscaledDeltaTime;
            float cfps = 1f / (float)time.ElapsedGameTime.TotalSeconds;

            ups.Enqueue(cups);
            while (ups.Count > FPS_COUNTER_SMOOTHING)
                ups.Dequeue();

            fps.Enqueue(cfps);
            while (fps.Count > FPS_COUNTER_SMOOTHING)
                fps.Dequeue();

            var mousePos = Input.MouseWorldPosition;
            int x = (int)(mousePos.X / Terrain.TILE_SIZE);
            int y = (int)(mousePos.Y / Terrain.TILE_SIZE);
            bool tt = Main.CurrentMap.InBounds(x, y);
            string tile = null;
            if (tt)
            {
                byte type = Main.CurrentMap.Terrain.GetTypeAt(x, y);
                byte c = Main.CurrentMap.Terrain.GetColourIDAt(x, y);

                tile = "Texture: {0}, Colour ID: {1}".Form(type, c);
            }

            string[] toDraw = new string[]
            {
                "UPS: " + Math.Round(ups.Average()),
                "FPS: " + Math.Round(fps.Average()),
                "Character Count: " + Main.CurrentMap.Characters.CharacterCount,
                "Pending Path Requests: " + Main.CurrentMap.Pathing.TotalPendingRequests,
                "Culled Tile Area: " + Main.CurrentMap.TileDrawBounds.Width * Main.CurrentMap.TileDrawBounds.Height
            };

            Vector2 drawPos = new Vector2(5, 5);
            SpriteFont font = Main.DefaultUIFont;
            Color colour = Color.Black;

            foreach (var item in toDraw)
            {
                var size = font.MeasureString(item);
                spr.DrawString(font, item, drawPos, colour);
                drawPos.Y += size.Y;
            }
        }
    }
}
