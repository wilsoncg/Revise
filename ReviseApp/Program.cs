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
            Output(Revise1.Fib().Take(10));
            Output(Library.fibs.Take(10));
            long s = 213;
            Console.WriteLine("Make change for 213p = " + Output(Revise1.MakeChange(s)));
            Console.WriteLine(Revise1.TriangleArea(21, 32, 87));

            var revise2 = new Revise2();
            Console.WriteLine("TapeEquilibrium "+ revise2.TapeEquilibrium(new[] { 3,1,2,4,3 }));
            Console.WriteLine("FrogJump " + revise2.FrogJump(10, 85, 30));
            Console.WriteLine("FrogJump " + revise2.FrogJump(10, 10, 1));
            Console.WriteLine("PermMissingElem " + revise2.PermMissingElem(new[] { 2, 3, 1, 5 }));
            Console.WriteLine("MissingInt " + revise2.MissingInteger(new[] { 2 }));
            Console.WriteLine("MissingInt " + revise2.MissingInteger(new[] { -1, -3 }));

            Console.ReadKey();
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
}
