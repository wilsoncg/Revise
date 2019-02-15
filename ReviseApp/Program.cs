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
            Console.WriteLine();
            Output(Library.fibs.Take(10));

            Console.ReadKey();
        }

        static void Output(IEnumerable<int> seq)
        {
            Console.WriteLine(string.Join(",", seq));
        }

        static IEnumerable<int> fib()
        {
            var first = 0;
            var second = 1;

            yield return first;
            yield return second;

            while (true)
            {
                (first, second) = (second, first + second);
                yield return second;
            }
        }

        static IEnumerable<(int coin, int howMany)> makeChange(int amount)
        {
            var coins = new[] { 1, 2, 5, 10, 20, 50, 100 };
            var largestCoins = coins.OrderByDescending(x => x);

            Enumerable
                    .Empty<(int coin, int howMany)>()
                    .Aggregate(amount, (a, b) => {
                        var largest = largestCoins.First(x => (x <= amount));
                        return ()
                    });

            var running = amount;
            while(true)
            {
                var coin = largestCoins.First(x => (x <= running));
                var running = running - coin;
                
                yield return (coin, 1);
            }
        }
    }
}
