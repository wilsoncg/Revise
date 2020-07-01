using Fsharp_ReviseApp;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        int hoarePartition<T>(T[] A, int lo, int hi, Comparer<T> comparer)
        {
            var pivotPoint = lo + (hi - lo) / 2;
            var pivot = A[pivotPoint];
            var i = lo - 1;
            var j = hi + 1;

            while(true)
            {
                do
                {
                    i++;
                }
                while (comparer.Compare(A[i], pivot) < 0);

                do
                {
                    j--;
                }
                while (comparer.Compare(A[j], pivot) > 0);

                if (i >= j)
                    return j;

                var x = A[i];
                var y = A[j];
                A[i] = y;
                A[j] = x;
                // swap A[i], A[j]
            }
        }

        T[] nonFunctionalSort<T>(T[] input, int lo, int hi) where T : IComparable<T>
        {
            if (lo < hi)
            {
                var p = hoarePartition(input, lo, hi, Comparer<T>.Default);
                nonFunctionalSort(input, lo, p);
                nonFunctionalSort(input, p + 1, hi);
            }

            return input;
        }

        public T[] ImperativeQuickSort<T>(IList<T> input) where T : IComparable<T> 
        {
            if (input.Count == 0)
                return input.ToArray();

            return nonFunctionalSort(input.ToArray(), 0, input.Count - 1);
        }
    }

    [TestClass]
    public class Revise_QuickSortTests
    {
        int[] Time(Func<int[]> f, string friendlyName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var r = f();
            stopwatch.Stop();

            Trace.WriteLine($"{friendlyName} took {stopwatch.ElapsedMilliseconds} ms");
            return r;
        }

        int[] Time(Func<int[], int[]> f, int[] arg, string friendlyName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var r = f(arg);
            stopwatch.Stop();

            Trace.WriteLine($"{friendlyName} took {stopwatch.ElapsedMilliseconds} ms");
            return r;
        }

        [TestMethod]
        public void Test_CSharp_Sort()
        {
            Func<int[]> gen = () => Utility.Generate(10_000).ToArray();
            var input = Time(gen, "Generate");

            Func<int[], int[]> sort = x => new Revise_QuickSort().Sort(x).ToArray();
            var sorted = Time(sort, input, "QuickSort with concat");

            Func<int[], int[]> frameworkSort = x => x.OrderBy(y => y).ToArray();
            var frameworkSorted = Time(frameworkSort, input, "Framework sort");

            CollectionAssert.AreEqual(frameworkSorted, sorted);
        }

        [TestMethod]
        public void Test_CSharp_Sort_WithYield()
        {
            var input = Utility.Generate(10_000).ToArray();

            Func<int[], int[]> sort = x => x.QuickSort().ToArray();
            var sorted = Time(sort, input, "QuickSort with yield");

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
        public void Test_ImperativeSort()
        {
            var input = Utility.Generate(10_0000).ToArray();

            var sorted = Library.qsort(ListModule.OfSeq(input)).ToArray();
            var sorted2 = new Revise_QuickSort().ImperativeQuickSort(input);

            CollectionAssert.AreEqual(sorted2, sorted);
        }

        [TestMethod]
        public void Test_ImperativeSort_4Elements()
        {
            var input = new[] { 4, 3, 2, 1 };
            var actual = new Revise_QuickSort().ImperativeQuickSort(input);
            var expected = input.OrderBy(x => x).ToArray();

            CollectionAssert.AreEqual(
                actual, 
                expected,
                $"Expected [{string.Join(",", expected)}] got [{string.Join(",", actual)}]");
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
