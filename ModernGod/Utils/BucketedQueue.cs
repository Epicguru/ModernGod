using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Utils
{
    public class BucketedQueue<T>
    {
        // A very simple queue class that enqueues into the most empty 'bucket' but dequeues from a specific bucket.
        // All buckets are independent and are therefore safe to access from different threads.
        public int BucketCount
        {
            get
            {
                return queues.Length;
            }
        }
        public int TotalCount { get; private set; }

        private Queue<T>[] queues;
        private object key = new object();

        public BucketedQueue(int buckets)
        {
            if (buckets < 1)
                throw new ArgumentException("buckets", "The number of buckets cannot be less than one!");

            queues = new Queue<T>[buckets];
            for (int i = 0; i < buckets; i++)
            {
                queues[i] = new Queue<T>();
            }
        }

        public void Enqueue(T item)
        {
            lock (key)
            {
                // Find the emptiest queue.
                Queue<T> lowest = queues[0];

                for (int i = 0; i < BucketCount; i++)
                {
                    var q = queues[i];
                    if (q.Count < lowest.Count)
                    {
                        lowest = q;
                    }
                }

                if (!lowest.Contains(item))
                {
                    lowest.Enqueue(item);
                    TotalCount++;
                }
            }
        }

        public T Dequeue(int bucket)
        {
            lock (key)
            {
                if (CountIn(bucket) <= 0)
                    throw new Exception("Bucket number " + bucket + " is empty, or the bucket index is out of range. (" + BucketCount + "total buckets.)");

                TotalCount--;
                return queues[bucket].Dequeue();
            }            
        }

        public int CountIn(int bucket)
        {
            if (bucket < 0 || bucket >= BucketCount)
                return -1;

            return queues[bucket].Count;
        }

        public void Clear()
        {
            lock (key)
            {
                foreach (var q in queues)
                {
                    q.Clear();
                }
            }
        }
    }
}
