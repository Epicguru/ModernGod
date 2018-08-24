using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModernGod.Debugging;
using ModernGod.Textures;
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
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            CurrentMap = new Map("Jamesville", 100, 100);
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
            Camera.Position = new Vector2(50 * 16);

            if(CurrentMap != null)
                CurrentMap.Update();

            base.Update(gameTime);
            Time.UpdateEnded();
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
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());

            if(CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            spriteBatch.End();

            // Draw UI.
            spriteBatch.Begin();
            Debug.DrawStats(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
