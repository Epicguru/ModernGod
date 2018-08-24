using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ModernGod
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom
        {
            get
            {
                return this._zoom;
            }
            set
            {
                this._zoom = Math.Max(Math.Min(value, 10), 0.005f);
            }
        }
        private float _zoom = 1f;
        private Matrix _transform;

        public void UpdateMatrix(GraphicsDevice graphicsDevice)
        {
            _transform =
              Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                         Matrix.CreateRotationZ(MathHelper.ToRadians(-Rotation)) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
        }

        public Matrix GetMatrix()
        {
            return _transform;
        }
    }
}
