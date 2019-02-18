using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise2
    {
        // https://codility.com/demo/take-sample-test/tape_equilibrium
        public int TapeEquilibrium(int[] A)
        {
            var left = A.Take(1).Sum();
            var right = A.Skip(1).Sum();

            var accum = new TapeEquilibriumAccum { Left = left, Right = right, min = Math.Abs(left - right) };

            var list = A.Skip(1).Take(A.Length - 2);
            return 
                list.Aggregate(
                accum,
                (a, x) => {
                    var l = (a.Left + x);
                    var r = (a.Right - x);
                    var difference = Math.Abs(l - r);
                    a.Left = l;
                    a.Right = r;

                    if (difference < a.min)
                        a.min = difference;

                    Console.WriteLine($"|{a.Left} - {a.Right}|={difference}");
                    return a;
                },
                a => a.min);
        }

        class TapeEquilibriumAccum
        {
            public int Left { get; set; }
            public int Right { get; set; }
            public int min { get; set; }
        }
    }
}
