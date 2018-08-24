using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ModernGod
{
    public static class Input
    {
        public static KeyboardState LastKeyState { get; private set; }
        public static KeyboardState CurrentKeyState { get; private set; }
        public static MouseState LastMouseState { get; private set; }
        public static MouseState CurrentMouseState { get; private set; }

        public static Vector2 MouseWorldPosition { get; private set; }
        public static Vector2 MouseScreenPosition { get; private set; }

        public static void Update()
        {
            LastKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            MouseScreenPosition = CurrentMouseState.Position.ToVector2();

            // The world position is found by translating screen space using the camera matrix.
            MouseWorldPosition = Vector2.Transform(MouseScreenPosition, Matrix.Invert(Main.Camera.GetMatrix()));

        }

        public static bool KeyDown(Keys key)
        {
            return LastKeyState.IsKeyUp(key) && CurrentKeyState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return CurrentKeyState.IsKeyDown(key);
        }

        public static bool LeftMouseDown()
        {
            return LastMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool LeftMouseUp()
        {
            return LastMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released;
        }

        public static bool RightMouseDown()
        {
            return LastMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool RightMouseUp()
        {
            return LastMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Released;
        }

        public static bool LeftMousePressed()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool RightMousePressed()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed;
        }
    }
}
