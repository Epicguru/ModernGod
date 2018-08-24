using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace ModernGod
{
    public static class Time
    {
        private static Stopwatch watch = new Stopwatch();

        public const float MAX_DELTA_TIME = 1f;
        public const float MIN_DELTA_TIME = 1f / 1000f;

        public static ulong ElapsedFrames { get; private set; }
        public static float DeltaTime { get; private set; }
        public static float UnscaledDeltaTime { get; private set; }
        public static float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (value < 0f)
                    value = 0;

                _scale = value;
            }
        }
        private static float _scale = 1f;

        public static void UpdateStarted(GameTime time)
        {
            //float elapsed = (float)watch.Elapsed.TotalSeconds;
            //watch.Restart();
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;
            elapsed = Math.Min(elapsed, MAX_DELTA_TIME);
            elapsed = Math.Max(elapsed, MIN_DELTA_TIME);

            UnscaledDeltaTime = elapsed;
            DeltaTime = UnscaledDeltaTime * Scale;
        }

        public static void UpdateEnded()
        {
            ElapsedFrames++;
        }
    }
}
