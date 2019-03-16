using Fsharp_ReviseApp;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise_QuickSort
    {
        public IEnumerable<T> Sort<T>(IEnumerable<T> input) where T : IComparable<T>
        {
            // quicksort []     = []
            // quicksort (x:xs) = quicksort ys ++ [x] ++ quicksort zs
            //          where
            //          ys = [a | a <- xs, a <= x]
            //          zs = [b | b <- xs, b > x]

            if (!input.Any())
                return Enumerable.Empty<T>();

            var x = input.Take(1).First();
            var xs = input.Skip(1);

            var ys = xs.Where(a => a.CompareTo(x) <= 0);
            var zs = xs.Where(b => b.CompareTo(x) > 0);
            
            return Sort(ys).Concat(new[] { x }).Concat(Sort(zs));
        }

        //public static IEnumerable<int> operator +(IEnumerable<int> a, IEnumerable<int> b)
        //{
        //    return a.Concat(b);
        //}
    }

    [TestClass]
    public class Revise_QuickSortTests
    {
        [TestMethod]
        public void Test_CSharp()
        {
            var input = new[] { 3, 6, 4, 1, 5, 2 };
            var sorted = new Revise_QuickSort().Sort(input).ToList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, sorted);
        }

        [TestMethod]
        public void Test_FSharp()
        {
            var input = new[] { 3, 6, 4, 1, 5, 2 };
            var sorted = Library.qsort(ListModule.OfSeq(input)).ToList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, sorted);
        }
    }
}
