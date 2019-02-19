﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise2
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

        // https://app.codility.com/programmers/lessons/4-counting_elements/missing_integer/
        public int MissingInteger(int[] A)
        {
            if (!A.Any())
                return 1;
            var a = A.OrderBy(x => x);

            var r = Enumerable.Range(1, A.Length + 1).Except(a).First();
            return r;
        }

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
    public class Revise2Tests
    {
        [TestMethod]
        public void MissingInteger_basic()
        {
            Assert.AreEqual(4, new Revise2().MissingInteger(new[] { 1, 2, 3 }));
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { -1, -3 }));
            Assert.AreEqual(5, new Revise2().MissingInteger(new[] { 1, 3, 6, 4, 1, 2 }));
        }

        [TestMethod]
        public void MissingInteger_extreme_single()
        {
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { 2 }));
        }

        [TestMethod]
        public void MissingInteger_failed()
        {
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { -5, -4, -3, -2, -1 }));
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { 0, 2, 3, 5, 6, 7 }));
        }
    }
}
