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
            var f1 = fib().Take(10);
            Console.WriteLine(string.Join(",", f1));
            Console.WriteLine();
            var f2 = Library.fibs.Take(10);
            Console.WriteLine(string.Join(",", f2));
            Console.ReadKey();
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
    }
}
