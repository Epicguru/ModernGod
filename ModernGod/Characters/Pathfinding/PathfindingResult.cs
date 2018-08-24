﻿
namespace ModernGod.Pathfinding
{
    public enum PathfindingResult : byte
    {
        SUCCESSFUL,
        CANCELLED,
        ERROR_START_OUT_OF_BOUNDS,
        ERROR_END_OUT_OF_BOUNDS,
        ERROR_START_IS_END,
        ERROR_PATH_TOO_LONG,
        ERROR_START_NOT_WALKABLE,
        ERROR_END_NOT_WALKABLE,
        ERROR_INTERNAL
    }
}
