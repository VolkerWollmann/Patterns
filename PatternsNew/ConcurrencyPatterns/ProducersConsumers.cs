using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Patterns.ConcurrencyPatterns
{
    public class ProducersConsumers
    {
        static Queue<int> buffer = new Queue<int>();
        static int bufferSize = 5;
        static object lockObject = new object();

        static void Producer()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (lockObject)
                {
                    while (buffer.Count >= bufferSize)
                    {
                        Console.WriteLine("Buffer is full. Producer is waiting...");
                        Monitor.Wait(lockObject);
                    }

                    buffer.Enqueue(i);
                    Console.WriteLine($"Produced: {i}");
                    Monitor.PulseAll(lockObject);
                }

                Thread.Sleep(100); // Simulate some work
            }
        }

        static void Consumer()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (lockObject)
                {
                    while (buffer.Count == 0)
                    {
                        Console.WriteLine("Buffer is empty. Consumer is waiting...");
                        Monitor.Wait(lockObject);
                    }

                    int item = buffer.Dequeue();
                    Console.WriteLine($"Consumed: {item}");
                    Monitor.PulseAll(lockObject);
                }

                Thread.Sleep(200); // Simulate some work
            }
        }
        public static void RunProducersConsumers()
        {
            var producerTask = Task.Run(Producer);
            var consumerTask = Task.Run(Consumer);

            Task.WhenAll(producerTask, consumerTask).Wait();
        }
    }

}
