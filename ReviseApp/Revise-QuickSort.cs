using Fsharp_ReviseApp;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://weblogs.asp.net/dixin/category-theory-via-c-sharp-23-knowing-the-cost

namespace System.Collections.Generic
{
    public static class Ext
    {
        public static IEnumerable<T> QuickSort<T>(this IEnumerable<T> values)
                where T : IComparable
        {
            if (!values.Any())
                yield break;

            var x = values.First();
            var xs = values.Skip(1);

            foreach (var l in xs.Where(i => i.CompareTo(x) < 0).QuickSort())
                yield return l;

            yield return x;

            foreach (var r in xs.Where(i => i.CompareTo(x) >= 0).QuickSort())
                yield return r;
        }
    }
}

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
                return input;

            var x = input.First();
            var xs = input.Skip(1);

            var ys = xs.Where(a => a.CompareTo(x) <= 0);
            var zs = xs.Where(b => b.CompareTo(x) > 0);
            
            return Sort(ys).Concat(new[] { x }).Concat(Sort(zs));
        }       
    }

    [TestClass]
    public class Revise_QuickSortTests
    {
        [TestMethod]
        public void Test_CSharp_Sort()
        {
            var input = Utility.Generate(10_000).ToArray();
            var sorted = new Revise_QuickSort().Sort(input).ToArray();

            CollectionAssert.AreEqual(input.OrderBy(x => x).ToArray(), sorted);
        }

        [TestMethod]
        public void Test_CSharp_Sort_WithYield()
        {
            var input = Utility.Generate(10_000).ToArray();
            var sorted = input.QuickSort().ToArray();

            CollectionAssert.AreEqual(input.OrderBy(x => x).ToArray(), sorted);
        }

        [TestMethod]
        public void Test_FSharp()
        {
            var input = new[] { 3, 6, 4, 1, 5, 2 };
            var input2 = Utility.Generate(10_0000).ToArray();
            var sorted = Library.qsort(ListModule.OfSeq(input)).ToArray();
            var sorted2 = Library.qsort(ListModule.OfSeq(input2)).ToArray();

            CollectionAssert.AreEqual(input.OrderBy(x => x).ToArray(), sorted);
            var frameworkSort = input2.OrderBy(x => x).ToArray();
            CollectionAssert.AreEqual(frameworkSort, sorted2);
        }

        [TestMethod]
        public void Test_FrameworkSort()
        {
            // no assertion, just using VS Test runner to see timing
            var input = Utility.Generate(10_0000).ToArray();

            Assert.IsTrue(input.OrderBy(x => x).ToArray().Any());
        }
    }
}
