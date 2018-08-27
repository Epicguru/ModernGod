using ModernGod.Logging;
using ModernGod.Pathfinding;
using ModernGod.Utils;
using ModernGod.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModernGod.Characters.Pathfinding
{
    public class PathManager : IDisposable
    {
        public Map Map { get; private set; }
        public int ThreadCount { get; private set; }
        public bool Running { get; private set; }
        public int TotalPendingRequests
        {
            get
            {
                return requests == null ? 0 : requests.TotalCount;
            }
        }

        private BucketedQueue<PathingRequest> requests;

        private Thread[] threads;
        private Pathing[] pathers;

        public PathManager(int threadCount, Map map)
        {
            if(threadCount <= 0)            
                throw new ArgumentException("The thread count must be at least one!");

            this.Map = map ?? throw new ArgumentNullException("map", "The map object value cannot be null!");
            this.ThreadCount = threadCount;
        }

        public void MakeRequest(PathingRequest request)
        {
            if (request == null)
                return;

            requests.Enqueue(request);
        }

        public void Start()
        {
            // Need to create all the threads
            if (Running)
                return;
            Running = true;

            requests = new BucketedQueue<PathingRequest>(ThreadCount);
            threads = new Thread[ThreadCount];
            pathers = new Pathing[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
            {
                Thread t = new Thread(Run);
                threads[i] = t;
                t.Name = "Pathfinding #" + i;

                Pathing p = new Pathing();
                pathers[i] = p;

                t.Start(i);
            }
        }

        private void Run(object objNum)
        {
            int number = (int)objNum;
            Console.WriteLine("Starting pathfinding thread numer " + number);

            Pathing p = pathers[number];

            // Settings here...
            const int SLEEP_TIME = 3;

            while (Running)
            {
                // Process stuff here.
                if(requests.CountIn(number) > 0)
                {
                    var req = requests.Dequeue(number);

                    if (req.Cancelled)
                    {
                        // Has been cancelled, bye then...
                        req.InvokeCompleted(PathfindingResult.CANCELLED, null);
                        continue;
                    }
                    else
                    {
                        List<PNode> path;
                        var result = p.Run(req.Start.X, req.Start.Y, req.End.X, req.End.Y, Map, out path);

                        // Give the results...
                        req.InvokeCompleted(result, path);
                    }
                }
                else
                {
                    Thread.Sleep(SLEEP_TIME);
                }
            }
        }

        public void Shutdown()
        {
            // Stop all the threads.
            if (!Running)
                return;
            Running = false;
        }

        public void Dispose()
        {
            if (Running)
            {
                // Shutdown before disposing...
                Shutdown();
            }

            // Dispose...
            threads = null;
            for (int i = 0; i < ThreadCount; i++)
            {
                pathers[i].Dispose();
            }
        }
    }
}
