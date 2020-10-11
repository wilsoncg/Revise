using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Codility06_Sorting
    {
        public class IntersectionAccumulator
        {
            public int numIntersections;
            public Tuple<int, int> Disc;
        }

        IEnumerable<Tuple<T, int>> WithIndex<T>(IEnumerable<T> list)
        {
            var e = list.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                yield return Tuple.Create(e.Current, i);
                i++;
            }
        }

        public class ValueWithIndex<T>
        {
            public T value { get; set; }
            public int index { get; set; }
        }

        IEnumerable<ValueWithIndex<T>> ToValueWithIndex<T>(IEnumerable<T> list)
        {
            var e = list.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                yield return new ValueWithIndex<T> { value = e.Current, index = i };
                i++;
            }
        }

        public int NumberOfIntersections(int[] A)
        {            
            if (A.Length <= 1)
                return 0;

            var sortedByRadiusDescending = 
                WithIndex(A)
                .OrderByDescending(a => a.Item1);

            var first = sortedByRadiusDescending.First();
            var xs = sortedByRadiusDescending.Skip(1);

            var r = 
                xs.Aggregate(
                    new IntersectionAccumulator { Disc = first },
                    (j, k) => {
                        var rightEdgeOfj = j.Disc.Item2 + j.Disc.Item1;
                        var leftEdgeOfk = k.Item2 - k.Item1;
                        var rightEdgeOfk = k.Item2 + k.Item1;

                        // check rightEdge of j crosses with leftEdge of k
                        if (leftEdgeOfk <= rightEdgeOfj || 
                            rightEdgeOfj >= rightEdgeOfk)
                            j.numIntersections++;

                        j.Disc = k;
                        return j;
                    });

            if (r.numIntersections > 10000000)
                return -1;
            return r.numIntersections;
        }
        
        public int MaxProductOfThree(int[] A)
        {
            var sorted = A.OrderByDescending(a => a).ToArray();
            
            return Math.Max(
                sorted[0] * sorted[1] * sorted[2], 
                sorted[0] * sorted[A.Length - 1] * sorted[A.Length -2]);
        }

        public int Distinct(int[] A)
        {
            var d = new Dictionary<int, bool>();
            foreach(var x in A)
            {
                if(!d.ContainsKey(x))
                {
                    d.Add(x, true);
                }                
            }
            return d.Count;
        }

        public int Distinct_WithLinq(int[] A)
        {
            return A.Distinct().Count();
        }

        public class TriangleAccum<T>
        {
            public List<T> list {get; set;}
            public List<T> result { get; set; }
            public int i { get; set; }
        }

        public int TriangleDetection(int[] A)
        {
            if (A.Length < 3)
                return 0;

            var a = ToValueWithIndex<int>(A).OrderByDescending(x => x.value).ToList();

            var seed = new TriangleAccum<ValueWithIndex<int>> { 
                list = a, 
                result = new List<ValueWithIndex<int>>(), 
                i = 0 
            };
            a.Aggregate(
                seed,
                (s, v) => {
                    if (s.i + 2 > s.list.Count - 1)
                        return s;

                    var r = v;
                    var q = s.list.ElementAt(s.i + 1);
                    var p = s.list.ElementAt(s.i + 2);
                    if (q.value + p.value > r.value &&
                        q.value + r.value > p.value &&
                        r.value + p.value > q.value)
                        s.result.Add(v);

                    s.i += 1;
                    return s; 
                });

            return seed.result.Count > 0 ? 1 : 0;
        }
    }

    [TestClass]
    public class Codility06_SortingTests
    {
        Codility06_Sorting sorting = new Codility06_Sorting();

        [TestMethod]
        public void Distinct_SampleTest()
        {
            var input = new[] { 2, 1, 1, 2, 3, 1 };
            var r = sorting.Distinct_WithLinq(input);
            var r1 = sorting.Distinct(input);

            Assert.AreEqual(3, r);
            Assert.AreEqual(3, r1);
        }

        [TestMethod]
        public void MaxProductOfThree_SampleTest()
        {
            var input = new[] { -3, 1, 2, -2, 5, 6 };
            var r1 = sorting.MaxProductOfThree(input);

            Assert.AreEqual(60, r1);
        }

        [TestMethod]
        public void MaxProductOfThree_Simple1()
        {
            var input = new[] { -5, 5, -5, 4 };
            var r1 = sorting.MaxProductOfThree(input);

            Assert.AreEqual(125, r1);
        }

        [TestMethod]
        public void MaxProductOfThree_Simple2()
        {
            var input = new[] { -5, 4, -5, 5 };
            var r1 = sorting.MaxProductOfThree(input);

            Assert.AreEqual(-5 * -5 * 5, r1);
        }

        [TestMethod]
        public void Intersections_ForSample()
        {
            var input = new[] { 1, 5, 2, 1, 4, 0 };
            var r = sorting.NumberOfIntersections(input);

            Assert.AreEqual(11, r);
        }

        [TestMethod]
        public void Intersections_ForOnePoint_AndTwoDiscs()
        {
            var input = new[] { 0, 0, 1 };
            var r = sorting.NumberOfIntersections(input);

            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void Intersections_ForTwoDiscs()
        {
            var input = new[] { 2, 1 };
            var r = sorting.NumberOfIntersections(input);

            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void Intersections_ForTwoDiscs_AndOnePoint()
        {
            var input = new[] { 2, 1, 0 };
            var r = sorting.NumberOfIntersections(input);

            Assert.AreEqual(2, r);
        }

        [TestMethod]
        public void Intersections_ShouldBeThree()
        {
            var input = new[] { 1, 5, 2 };
            var r = sorting.NumberOfIntersections(input);

            Assert.AreEqual(3, r);
        }

        [TestMethod]
        public void Triangle_DetectionShouldReturnOne()
        {
            var input = new[] { 10, 2, 5, 1, 8, 20 };
            var r = sorting.TriangleDetection(input);

            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void Triangle_DetectionShouldReturnZero()
        {
            var input = new[] { 10, 50, 5, 1 };
            var r = sorting.TriangleDetection(input);

            Assert.AreEqual(0, r);
        }
    }
}
