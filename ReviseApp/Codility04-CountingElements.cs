using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReviseApp
{
    public static class Ext
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(
            this IEnumerable<T> items,
            Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }

    public class Codility04_CountingElements
    {
        public int PermCheck(int[] A)
        {
            var notPerm = 
                Enumerable
                .Range(1, A.Length)
                .Except(A)
                .Any() ? true : false;

            return notPerm ? 0 : 1;
        }

        public int[] MaxCounters(int N, int[] A)
        {
            Func<int, Dictionary<int, int>> setCounters = counterValue =>
                Enumerable
                .Range(1, N)
                .ToDictionary(x => x, y => counterValue);
            Func<int, int> increase = x => x + 1;
            var max = 0;
            var runningMax = 0;

            var counters = setCounters(0);

            int n = 0;
            while(n < A.Length)
            {
                var ak = A[n];

                if (ak >= 1 && ak <= N)
                {
                    if (counters[ak] < max)
                        counters[ak] = max;

                    var x = counters[ak];
                    var counter = increase(x);
                    counters[ak] = counter;

                    runningMax = counter > runningMax ? counter : runningMax;
                }

                if(ak == N + 1)
                {
                    max = runningMax;
                }
                n++;
            }
            return counters.Select(x => x.Value < max ? max : x.Value).ToArray();
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

        // https://app.codility.com/programmers/lessons/4-counting_elements/frog_river_one/
        IEnumerable<Tuple<int, int>> WithIndex(IEnumerable<int> list)
        {
            var e = list.GetEnumerator();
            int i = 0;
            while(e.MoveNext())
            {
                yield return Tuple.Create(e.Current, i);
                i++;
            }
        }

        // O(N**N)
        public int FrogRiverOne_Scalable(int X, int[] A)
        {
            var toFind = Enumerable.Range(1, X);

            var r =
                toFind
                .Join(
                    WithIndex(A).OrderBy(x => x).DistinctBy(y => y.Item1),
                    outer => outer,
                    inner => inner.Item1,
                    (x, inner) => inner.Item2);

            var mismatch = r.Count() < toFind.Count();
            if (mismatch)
                return -1;
            
            return r.Max();
        }

        // O(N)
        public int FrogRiverOne_Performant(int X, int[] A)
        {
            var toFind = 
                Enumerable
                .Range(1, X)
                .ToDictionary(keySelector: x => x, elementSelector: y => -1);
            var leftToFindCount = X;
            int i = 0;

            foreach (var leaf in A)
            {
                var second = toFind[leaf];
                if (second == -1) { 
                    toFind[leaf] = i;
                    
                    if(leftToFindCount > 0)
                        leftToFindCount--;
                }
                i++;
            }

            if (leftToFindCount > 0)
                return -1;

            return toFind.Max(x => x.Value);
        }
    }

    [TestClass]
    public class Codility04_CountingElementsTests
    {
        private Codility04_CountingElements counting = new Codility04_CountingElements();

        [TestMethod]
        public void MissingInteger_basic()
        {
            Assert.AreEqual(4, counting.MissingInteger(new[] { 1, 2, 3 }));
            Assert.AreEqual(1, counting.MissingInteger(new[] { -1, -3 }));
            Assert.AreEqual(5, counting.MissingInteger(new[] { 1, 3, 6, 4, 1, 2 }));
        }

        [TestMethod]
        public void MissingInteger_extreme_single()
        {
            Assert.AreEqual(1, counting.MissingInteger(new[] { 2 }));
        }

        [TestMethod]
        public void MissingInteger_failed()
        {
            Assert.AreEqual(1, counting.MissingInteger(new[] { -5, -4, -3, -2, -1 }));
            Assert.AreEqual(1, counting.MissingInteger(new[] { 0, 2, 3, 5, 6, 7 }));
        }

        [TestMethod]
        public void Frog_CanCrossAfter6Seconds()
        {
            // river is 5 wide 
            var input = new[] { 1, 3, 1, 4, 2, 3, 5, 4 };
            Assert.AreEqual(6, counting.FrogRiverOne_Performant(5, input));
            Assert.AreEqual(6, counting.FrogRiverOne_Scalable(5, input));
        }

        [TestMethod]
        public void PermCheck_IsPermutation()
        {
            Assert.AreEqual(1, counting.PermCheck(new[] { 4, 1, 3, 2 }));
        }

        [TestMethod]
        public void PermCheck_IsNotPermutation()
        {
            Assert.AreEqual(0, counting.PermCheck(new[] { 4, 1, 3 }));
        }

        [TestMethod]
        public void MaxCounters_AllZero()
        {
            var input = new[] { 4, 4, 4 };

            CollectionAssert.AreEqual(
                new[] { 0, 0, 0, },
                counting.MaxCounters(3, input));
        }

        [TestMethod]
        public void MaxCounters_small()
        {
            var input = new[] { 2, 1, 1, 2, 1 };

            CollectionAssert.AreEqual(
                new[] { 3 }, 
                counting.MaxCounters(1, input));
        }

        [TestMethod]
        public void MaxCounters_FromSample()
        {
            var input = new[] { 3, 4, 4, 6, 1, 4, 4 };
            var r = counting.MaxCounters(5, input);

            CollectionAssert.AreEqual(
                new[] { 3, 2, 2, 4, 2 }, 
                r);
        }

        IEnumerable<int> Generate(int n, int maxValue)
        {
            var random = new Random();
            for(int i = 1; i < n; i ++)
            {
                yield return random.Next(maxValue);
            }
        }

        [TestMethod]
        public void MaxCountersIsPerformant_For_10_000_CounterOperations()
        {
            var N = 100_000;
            var input = Enumerable.Range(1, N).ToArray();
            int n = 0;
            foreach(var i in input)
            {
                if (i % 10 == 0)
                    input[n] = N + 1;

                n++;
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = counting.MaxCounters(N, input);
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.Elapsed.TotalSeconds < 6);
        }

        [TestMethod]
        public void MaxCountersIsPerformant_For_3_000_CounterOperations()
        {
            var N = 100_000;
            var input = Enumerable.Range(1, N).ToArray();
            int n = 0;
            foreach (var i in input)
            {
                if (i % 30 == 0)
                    input[n] = N + 1;

                n++;
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = counting.MaxCounters(N, input);
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.Elapsed.TotalSeconds < 4);
        }
    }
}
