using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModernGod.Logging;
using ModernGod.Textures;
using ModernGod.Utils;
using ModernGod.World;
using System;
using System.Threading;

namespace ModernGod
{
    public class Main : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static ContentManager ContentMangager
        {
            get
            {
                return Main.instance.Content;
            }
        }
        public static readonly Camera Camera = new Camera();
        public static SystemInfo SystemInfo;

        public TextureAtlas<byte> TerrainAtlas;
        public static SpriteFont DefaultUIFont;

        public static Map CurrentMap;

        private SpriteBatch spriteBatch;
        private static Main instance;

        public Main()
        {
            instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            base.Window.AllowUserResizing = true;
            Graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Collect some system info. All anonymous and never leaves the process.
            SystemInfo = new SystemInfo();
            SystemInfo.Collect();

            CurrentMap = new Map("Jamesville", 400, 400);
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultUIFont = Content.Load<SpriteFont>("Fonts/Default UI");

            CurrentMap.Init();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.UpdateStarted(gameTime);
            Input.Update();

            // Do all updating here!
            int state = Mouse.GetState().ScrollWheelValue;
            if(state >= 0)
            {
                Camera.Zoom = 1f + (0.0005f * state);
            }
            else
            {
                Camera.Zoom = 1f - (Math.Abs(state) * 0.0005f);
            }
            Camera.Position = new Vector2(200 * 16);

            if(CurrentMap != null)
                CurrentMap.Update();

            base.Update(gameTime);
            Time.UpdateEnded();

            var size = Window.ClientBounds.Size;
            bool apply = false;
            if(size.X != Graphics.PreferredBackBufferWidth)
            {
                Graphics.PreferredBackBufferWidth = size.X;
                apply = true;
            }
            if(size.Y != Graphics.PreferredBackBufferHeight)
            {
                Graphics.PreferredBackBufferHeight = size.Y;
                apply = true;
            }
            if (apply)
            {
                Logger.Log("Resized to " + Graphics.PreferredBackBufferWidth + "x" + Graphics.PreferredBackBufferHeight);
                Graphics.ApplyChanges();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw to custom render targets...
            if (CurrentMap != null)
                CurrentMap.TargetDraw(spriteBatch);

            // Draw game world.
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Camera.UpdateMatrix(this.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());

            if(CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            spriteBatch.End();

            // Draw UI.
            spriteBatch.Begin();
            Logger.DrawStats(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
