using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    static class Revise1
    {
        public static IEnumerable<int> Fib()
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

        public static IEnumerable<(int coin, int howMany)> MakeChange(int amount)
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

                        return acc.Add(
                            (coin, (left / coin)),
                            left % coin);
                    },
                    (a) => a.Change)
                    .Where(x => x.howMany >= 1);
            return r;
        }

        static double CircleArea(double radius)
        {
            return Math.PI * Math.Pow(radius, 2);
        }

        static double DegToRad(double degree)
        {
            // 1 rad = 180/pi
            return degree * (Math.PI / 180);
        }

        public static double TriangleArea(double a, double b, double C)
        {
            return (0.5 * a * b * Math.Sin(DegToRad(C)));
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
