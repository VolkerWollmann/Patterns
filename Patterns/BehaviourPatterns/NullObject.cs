using System;

// Source : https://en.wikipedia.org/wiki/Null_object_pattern
//          https://refactoring.guru/introduce-null-object
// The Null Object pattern replaces a null reference with an object that implements the
// expected interface and does nothing at all. The caller stops asking whether it has a
// collaborator and simply uses it, so the null check is written once - where the object
// is handed over - instead of being repeated at every single call site.

namespace Patterns.BehaviourPatterns
{
    public class NullObjectExample
    {
        /// <summary>
        /// What the service expects of a log.
        /// </summary>
        public interface ILog
        {
            void Write(string message);
        }

        /// <summary>
        /// A real implementation.
        /// </summary>
        public class ConsoleLog : ILog
        {
            public void Write(string message)
            {
                Console.WriteLine("   log: " + message);
            }
        }

        /// <summary>
        /// The 'Null Object': the same interface, with neutral behaviour. It carries no
        /// state, so a single shared instance serves everybody.
        /// </summary>
        public sealed class NullLog : ILog
        {
            public static readonly NullLog Instance = new NullLog();

            private NullLog()
            {
            }

            public void Write(string message)
            {
                // Doing nothing is the whole point.
            }
        }

        /// <summary>
        /// The client. Note what is absent: it never asks whether it has a log.
        /// </summary>
        public class OrderService
        {
            private readonly ILog _log;

            // The one place where absence is dealt with. A caller that does not care
            // about logging passes nothing and gets the null object ...
            public OrderService(ILog? log = null)
            {
                _log = log ?? NullLog.Instance;
            }

            public decimal Place(string article, int quantity, decimal unitPrice)
            {
                // ... so from here on there is no question whether _log is there.
                // Without the pattern every one of these lines would need an if.
                _log.Write($"order received: {quantity} x {article}");

                decimal total = quantity * unitPrice;

                _log.Write($"order total: {total}");

                return total;
            }
        }

        public static void NullObject()
        {
            Console.WriteLine("With a real log:");
            OrderService logging = new OrderService(new ConsoleLog());
            Console.WriteLine($"   total = {logging.Place("Bolt", 3, 1.5m)}");

            Console.WriteLine();
            Console.WriteLine("Without one - same code path, no null checks, no output:");
            OrderService silent = new OrderService();
            Console.WriteLine($"   total = {silent.Place("Bolt", 3, 1.5m)}");
        }
    }
}
