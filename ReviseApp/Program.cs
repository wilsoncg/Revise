using System;
using System.Collections.Generic;
using System.Linq;

using Fsharp_ReviseApp;

namespace ReviseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Output(fib().Take(10));
            Output(Library.fibs.Take(10));

            Output(makeChange(213));

            Console.ReadKey();
        }

        static void Output(IEnumerable<int> seq)
        {
            Console.WriteLine(string.Join(",", seq));
        }

        static void Output(IEnumerable<(int, int)> seq)
        {
            var r =
                seq.Aggregate(
                "",
                (a, b) => $"{a}({b.Item1}p, {b.Item2})");
            Console.WriteLine(r);
        }

        static IEnumerable<int> fib()
        {
            int current = 0, next = 1;

            while (true)
            {
                yield return current;
                (current, next) = (next, current + next);
            }
        }

        static int aggregateExample()
        {
            var list = new int[] { 2, 4, 8, 16, 32 };
            var sum = 0;
            return list.Aggregate(sum, (accumulator, x) => accumulator + x);
        }

        static IEnumerable<(int coin, int howMany)> makeChange(int amount)
        {
            var coins = new[] { 1, 2, 5, 10, 20, 50, 100 };
            var largestCoins = coins.OrderByDescending(x => x);
            var empty =
                Enumerable
                .Empty<(int coin, int howMany)>()
                .ToList();

            var r =
                largestCoins
                .Aggregate(
                    new Accum { Change = empty, Remaining = amount }, 
                    (Accum acc, int coin) => 
                    {
                        var left = acc.Remaining;
                        if (left == 0)
                            return acc;

                        if(coin == left)
                            return acc.Add((coin, 1), 0);

                        if ((left % coin) == 0)
                            return acc.Add((coin, (left / coin)), 0);

                        return acc.Add(
                            (coin, (left / coin)),
                            left % coin);
                    },
                    (a) => a.Change)
                    .Where(x => x.howMany >= 1);
            return r;
        }
    }

    class Accum
    {
        public List<(int coin, int howMany)> Change { get; set; }
        public int Remaining { get; set; }

        public Accum Add((int, int) c, int remaining)
        {
            Change.Add(c);
            Remaining = remaining;
            return this;
        }
    }
}
