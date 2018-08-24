﻿using ModernGod.Pathfinding;
using ModernGod.Pathfinding.HSPQ;
using ModernGod.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModernGod.Pathfinding
{
    public class Pathing
    {
        public const int MAX = 1024;
        public const float DIAGONAL_DST = 1.41421356237f;

        private FastPriorityQueue<PNode> open = new FastPriorityQueue<PNode>(MAX);
        private Dictionary<PNode, PNode> cameFrom = new Dictionary<PNode, PNode>();
        private Dictionary<PNode, float> costSoFar = new Dictionary<PNode, float>();
        private List<PNode> near = new List<PNode>();
        private bool left, right, below, above;

        public Pathing()
        {

        }

        public PathfindingResult Run(int startX, int startY, int endX, int endY, Map map, out List<PNode> path)
        {
            if (map == null)
            {
                path = null;
                return PathfindingResult.ERROR_INTERNAL;
            }
            if (!map.Initialized)
            {
                path = null;
                return PathfindingResult.ERROR_INTERNAL;
            }

            // Validate start and end points.
            if (!map.InBounds(startX, startY))
            {
                path = null;
                return PathfindingResult.ERROR_START_OUT_OF_BOUNDS;
            }
            if (!map.InBounds(endX, endY))
            {
                path = null;
                return PathfindingResult.ERROR_END_OUT_OF_BOUNDS;
            }
            if (!map.Interaction.IsTileWalkable(startX, startY))
            {
                path = null;
                return PathfindingResult.ERROR_START_NOT_WALKABLE;
            }
            if (!map.Interaction.IsTileWalkable(endX, endY))
            {
                path = null;
                return PathfindingResult.ERROR_END_NOT_WALKABLE;
            }

            // Clear everything up.
            Clear();

            var start = new PNode(startX, startY);
            var end = new PNode(endX, endY);

            // Check the start/end relationship.
            if (start.Equals(end))
            {
                path = null;
                return PathfindingResult.ERROR_START_IS_END;
            }

            // Add the starting point to all relevant structures.
            open.Enqueue(start, 0f);
            cameFrom[start] = start;
            costSoFar[start] = 0f;

            int count;
            while ((count = open.Count) > 0)
            {
                // Detect if the current open amount exceeds the capacity.
                // This only happens in very large open areas. Corridors and hallways will never cause this, not matter how large the actual path length.
                if (count >= MAX - 8)
                {
                    path = null;

                    // Clear everything up.
                    Clear();

                    return PathfindingResult.ERROR_PATH_TOO_LONG;
                }

                var current = open.Dequeue();

                if (current.Equals(end))
                {
                    // We found the end of the path!
                    path = TracePath(end);

                    // Clear everything up.
                    Clear();

                    return PathfindingResult.SUCCESSFUL;
                }

                // Get all neighbours (tiles that can be walked on to)
                var neighbours = GetNear(current, map);
                foreach (PNode n in neighbours)
                {
                    float newCost = costSoFar[current] + GetCost(current, n); // Note that this could change depending on speed changes per-tile.

                    if (!costSoFar.ContainsKey(n) || newCost < costSoFar[n])
                    {
                        costSoFar[n] = newCost;
                        float priority = newCost + Heuristic(current, n);
                        open.Enqueue(n, priority);
                        cameFrom[n] = current;
                    }
                }
            }

            // Clear everything up.
            Clear();

            path = null;
            return PathfindingResult.ERROR_INTERNAL;
        }

        private List<PNode> TracePath(PNode end)
        {
            List<PNode> path = new List<PNode>();
            PNode child = end;

            bool run = true;
            while (run)
            {
                PNode previous = cameFrom[child];
                path.Add(child);
                if (previous != null && child != previous)
                {
                    child = previous;
                }
                else
                {
                    run = false;
                }
            }

            path.Reverse();
            return path;
        }

        public void Clear()
        {
            costSoFar.Clear();
            cameFrom.Clear();
            near.Clear();
            open.Clear();
        }

        private float Heuristic(PNode a, PNode b)
        {
            // Gives a rough distance.
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private float GetCost(PNode a, PNode b)
        {
            // Only intended for neighbours.

            // Is directly horzontal
            if (Math.Abs(a.X - b.X) == 1 && a.Y == b.Y)
            {
                return 1;
            }

            // Directly vertical.
            if (Math.Abs(a.Y - b.Y) == 1 && a.X == b.X)
            {
                return 1;
            }

            // Assume that it is on one of the corners.
            return DIAGONAL_DST;
        }

        private List<PNode> GetNear(PNode node, Map map)
        {
            // Want to add nodes connected to the center node, if they are walkable.
            // This code stops the pathfinder from cutting corners, and going through walls that are diagonal from each other.

            near.Clear();

            // Left
            left = false;
            if (map.Interaction.IsTileWalkable(node.X - 1, node.Y))
            {
                near.Add(new PNode(node.X - 1, node.Y));
                left = true;
            }

            // Right
            right = false;
            if (map.Interaction.IsTileWalkable(node.X + 1, node.Y))
            {
                near.Add(new PNode(node.X + 1, node.Y));
                right = true;
            }

            // Above
            above = false;
            if (map.Interaction.IsTileWalkable(node.X, node.Y + 1))
            {
                near.Add(new PNode(node.X, node.Y + 1));
                above = true;
            }

            // Below
            below = false;
            if (map.Interaction.IsTileWalkable(node.X, node.Y - 1))
            {
                near.Add(new PNode(node.X, node.Y - 1));
                below = true;
            }

            // Above-Left
            if (left && above)
            {
                if (map.Interaction.IsTileWalkable(node.X - 1, node.Y + 1))
                {
                    near.Add(new PNode(node.X - 1, node.Y + 1));
                }
            }

            // Above-Right
            if (right && above)
            {
                if (map.Interaction.IsTileWalkable(node.X + 1, node.Y + 1))
                {
                    near.Add(new PNode(node.X + 1, node.Y + 1));
                }
            }

            // Below-Left
            if (left && below)
            {
                if (map.Interaction.IsTileWalkable(node.X - 1, node.Y - 1))
                {
                    near.Add(new PNode(node.X - 1, node.Y - 1));
                }
            }

            // Below-Right
            if (right && below)
            {
                if (map.Interaction.IsTileWalkable(node.X + 1, node.Y - 1))
                {
                    near.Add(new PNode(node.X + 1, node.Y - 1));
                }
            }

            return near;
        }
    }
}
