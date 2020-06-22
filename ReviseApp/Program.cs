using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fsharp_ReviseApp;

namespace ReviseApp
{
    class Program
    {
        CancellationTokenSource _cancellationTokenSource;
        BlockingCollection<QueuedItem> _queue = new BlockingCollection<QueuedItem>();
        static Timer _timerCheck;

        public Program()
        {
            _cancellationTokenSource = new CancellationTokenSource() { };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press Escape (Esc) key to quit. {0}", Environment.NewLine);
            Console.WriteLine("(R)un Examples");
            Console.WriteLine("(T)imer Example");

            ConsoleKeyInfo cki;
            Console.TreatControlCAsInput = true;
            do
            {
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.R)
                {
                    RunExamples();
                }
                if (cki.Key == ConsoleKey.T)
                {
                    new Program().TimerExample();
                }
                if (cki.Key == ConsoleKey.Escape)
                {
                    break;
                }
            } while (true);
        }

        void TimerExample()
        {
            _timerCheck =
                new Timer(
                    CheckForStuckItems,
                    null,
                    new TimeSpan(0, 0, 5),
                    new TimeSpan(0, 0, 5));
            Task.Factory.StartNew(DrainQueue, TaskCreationOptions.LongRunning);
        }

        void CheckForStuckItems(object o)
        {
            Console.WriteLine($"{DateTime.UtcNow} Timer fired, checking for items");

            //if (_queue.Any(x => x.IsFromDatabase))
            //{
            //    Console.WriteLine($"{DateTime.UtcNow} Skipping fetching items for queue");
            //    return;
            //}                

            List<QueuedItem> items = FetchQueuedItems();
            foreach (var item in items)
            {
                _queue.Add(item);
            }
        }

        List<QueuedItem> FetchQueuedItems()
        {
            var random = 
                Utility
                .Generate(500_000)
                .Select(x => new QueuedItem { Id = x, Value = "a", IsFromDatabase = (x % 2 == 0) })
                .ToList();
            Thread.Sleep(new TimeSpan(0, 0, 10));
            Console.WriteLine($"{DateTime.UtcNow} Adding another 500,000 items to queue");
            return random;
        }

        void DrainQueue()
        {
            foreach(var item in _queue.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                Thread.Sleep(300);
                _queue.Take();
            }
        }

        static void RunExamples()
        {
            Output(Revise1.Fib().Take(10));
            Output(Library.fibs.Take(10));
            long s = 213;
            Console.WriteLine("Make change for 213p = " + Output(Revise1.MakeChange(s)));
            Console.WriteLine(Revise1.TriangleArea(21, 32, 87));

            var counting = new Codility04_CountingElements();
            Console.WriteLine("MissingInt " + counting.MissingInteger(new[] { 2 }));
            Console.WriteLine("MissingInt " + counting.MissingInteger(new[] { -1, -3 }));
        }

        static void Output(IEnumerable<int> seq)
        {
            Console.WriteLine(string.Join(",", seq));
        }

        static void Output(IEnumerable<long> seq)
        {
            Console.WriteLine(string.Join(",", seq));
        }

        static string Output(IEnumerable<(int, long)> seq)
        {
            var r =
                seq.Aggregate(
                "",
                (a, b) => $"{a}({b.Item1}p, {b.Item2})");
            return r;
        }
    }

    class QueuedItem
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool IsFromDatabase { get; set; }
    }
}
