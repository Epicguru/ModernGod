using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }


        public Vector2Int(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2Int(int value)
        {
            this.X = value;
            this.Y = value;
        }

        public Vector2Int(Vector2Int other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        public override bool Equals(object obj)
        {
            if(obj is Vector2Int)
            {
                var other = (Vector2Int)obj;
                return this.X == other.X && this.Y == other.Y;
            }
            else if(obj is Vector2)
            {
                var other = (Vector2)obj;
                return this.X == other.X && this.Y == other.Y;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool AdjacentTo(Vector2Int other)
        {
            int x = Math.Abs(other.X - this.X);
            int y = Math.Abs(other.Y - this.Y);
            return x <= 1 && y <= 1;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Int operator *(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2Int operator *(Vector2Int a, int b)
        {
            return new Vector2Int(a.X * b, a.Y * b);
        }

        public static Vector2Int operator /(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2Int operator /(Vector2Int a, int b)
        {
            return new Vector2Int(a.X / b, a.Y / b);
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Vector2Int a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Int a, Vector2 b)
        {
            return !a.Equals(b);
        }

        public static implicit operator Vector2(Vector2Int vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static float Distance(Vector2Int a, Vector2Int b)
        {
            return (float)Math.Sqrt(SquareDistance(a, b));
        }

        public static float SquareDistance(Vector2Int a, Vector2Int b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        public static Vector2Int Lerp(Vector2Int a, Vector2Int b, float p, IntLerpMode mode = IntLerpMode.DEFAULT)
        {
            p = MathUtils.Clamp01(p);

            float x = MathUtils.Lerp(a.X, b.X, p);
            float y = MathUtils.Lerp(a.Y, b.Y, p);

            int X = 0;
            int Y = 0;

            switch (mode)
            {
                case IntLerpMode.DEFAULT:
                    X = (int)x;
                    Y = (int)y;
                    break;
                case IntLerpMode.FLOOR:
                    X = (int)Math.Floor(x);
                    Y = (int)Math.Floor(y);
                    break;
                case IntLerpMode.CEIL:
                    X = (int)Math.Ceiling(x);
                    Y = (int)Math.Ceiling(y);
                    break;
                case IntLerpMode.ROUND:
                    X = (int)Math.Round(x);
                    Y = (int)Math.Round(y);
                    break;
            }

            return new Vector2Int(X, Y);
        }
    }
}
