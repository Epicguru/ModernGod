﻿
using Microsoft.Xna.Framework;
using ModernGod.Pathfinding.HSPQ;
using ModernGod.Utils;
using System;

namespace ModernGod.Pathfinding
{
    public class PNode : FastPriorityQueueNode
    {

        public int X { get; private set; }
        public int Y { get; private set; }

        public PNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(PNode pn)
        {
            return new Vector2(pn.X, pn.Y);
        }

        public static implicit operator Vector2Int(PNode pn)
        {
            return new Vector2Int(pn.X, pn.Y);
        }

        public override bool Equals(object obj)
        {
            var other = (PNode)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X + Y * 7;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ")";
        }
    }
}
