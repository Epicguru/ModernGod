using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public static class MathUtils
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                float oldMax = max;
                max = min;
                min = oldMax;
            }

            if(value < min)            
                return min;
            
            if (value > max)
                return max;

            return value;
        }

        public static float Clamp01(float value)
        {
            return Clamp(value, 0f, 1f);
        }

        public static float Lerp(float a, float b, float p)
        {
            p = Clamp01(p);
            return a + (b - a) * p;
        }

        public static float LerpUnclamped(float a, float b, float p)
        {
            return a + (b - a) * p;
        }        
    }
}
