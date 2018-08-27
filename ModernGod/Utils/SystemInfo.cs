using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public class SystemInfo
    {
        // All default values below are guesses for the average modern system.
        // Data collected by this class is anonymous and will not be sent to anyone, anywhere.
        // Data collected is used to optimize per-system, but is not necessary for the game to run.

        /// <summary>
        /// The number of logical processing units. This is 8 on modern CPUs, 4 on slightly older CPUs and may be 2 or even 1 on old systems.
        /// Some people even own multiple CPUs with crazy high processing unit counts, like 64 or 128. Like why, man.
        /// </summary>
        public int ProcessorCount { get; private set; } = 4;

        /// <summary>
        /// Is the game running as a 64-bit executable?
        /// </summary>
        public bool IsRunning64 { get; private set; } = false;

        /// <summary>
        /// Is this machine running a 64-bit OS?
        /// </summary>
        public bool Is64OS { get; private set; } = false;

        /// <summary>
        /// The directory containing the executable file that is running this game.
        /// </summary>
        public string ExecutionPath { get; private set; } = null;

        public void Collect()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                ProcessorCount = Environment.ProcessorCount;
                IsRunning64 = Environment.Is64BitProcess;
                Is64OS = Environment.Is64BitOperatingSystem;
                ExecutionPath = Environment.CurrentDirectory;
            }
            catch (Exception e)
            {
                Logging.Logger.LogError("Failed collecting some or all system info...");
                Logging.Logger.LogError(e);
            }
            finally
            {
                watch.Stop();
                Logging.Logger.Log("Finished collecting system info in " + watch.Elapsed.TotalSeconds + " seconds.");

                Print("Processor Count", ProcessorCount);
                Print("Is 64 Process", IsRunning64);
                Print("Is 64 OS", Is64OS);
                Print("Exec Path", ExecutionPath);
            }
        }

        private void Print(string key, object value)
        {
            Logging.Logger.Log("  >" + key + ": " + value, ConsoleColor.DarkCyan);
        }
    }
}
