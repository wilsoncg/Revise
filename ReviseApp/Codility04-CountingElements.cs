using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Assert.AreEqual(6, counting.FrogRiverOne_Scalable(5, input));
        }
    }
}
