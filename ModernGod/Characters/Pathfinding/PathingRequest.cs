using ModernGod.Pathfinding;
using ModernGod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Characters.Pathfinding
{
    public class PathingRequest
    {
        public delegate void UponRequestCompleted(PathfindingResult result, List<PNode> path);

        public Vector2Int Start;
        public Vector2Int End;
        public UponRequestCompleted UponCompleted { get; private set; }

        public bool Completed { get; private set; }
        public bool Cancelled { get; set; }

        public PathingRequest(int sx, int sy, int ex, int ey, UponRequestCompleted uponCompleted)
        {
            this.Start = new Vector2Int(sx, sy);
            this.End = new Vector2Int(ex, ey);
            this.UponCompleted = uponCompleted;
        }

        public void InvokeCompleted(PathfindingResult res, List<PNode> path)
        {
            Completed = true;

            if (UponCompleted != null)
                UponCompleted.Invoke(res, path);
        }
    }
}
