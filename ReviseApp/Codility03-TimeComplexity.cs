using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Codility03_TimeComplexity
    {
        // https://app.codility.com/programmers/lessons/3-time_complexity/frog_jmp/
        public double FrogJump(int x, int y, int d)
        {
            var r = (y - x) / d;
            if ((r * d + x) < y)
                return r + 1;

            return r;
        }

        // https://app.codility.com/programmers/lessons/3-time_complexity/perm_missing_elem/
        public int PermMissingElem(int[] A)
        {
            return Enumerable.Range(1, A.Length + 2).Except(A).First();
        }

        // https://app.codility.com/programmers/lessons/3-time_complexity/tape_equilibrium/
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

                    //Console.WriteLine($"|{a.Left} - {a.Right}|={difference}");
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

    [TestClass]
    public class Codility03_TimeComplexityTests
    {
        Codility03_TimeComplexity timeComplexity = new Codility03_TimeComplexity();

        [TestMethod]
        public void FrogJump()
        {
            Assert.AreEqual(3, timeComplexity.FrogJump(10, 85, 30));
        }

        [TestMethod]
        public void TapeEquilibrium()
        {
            Assert.AreEqual(1, timeComplexity.TapeEquilibrium(new[] { 3, 1, 2, 4, 3 }));
        }

        [TestMethod]
        public void MissingElement()
        {
            Assert.AreEqual(4, timeComplexity.PermMissingElem(new[] { 2, 3, 1, 5 }));
        }
    }
}
