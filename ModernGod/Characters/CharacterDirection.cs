using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Characters
{
    public enum CharacterDirection : byte
    {
        RIGHT = 0,
        LEFT = 1,
        TOWARDS_CAM = 2,
        AWAY_CAM = 3
    }
}
