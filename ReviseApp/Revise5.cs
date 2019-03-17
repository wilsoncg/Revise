using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise5
    {
        public int NumSquareRoots(int A, int B)
        {
            if ((B - A) < 0)
                return 0;

            var range = 
                (B - A) > 0 ? 
                Enumerable.Range(A, B - A) : 
                new[] { A };
            return roots(range, 0).Item2;
        }

        Tuple<IEnumerable<int>, int> roots(IEnumerable<int> xs, int depth)
        {
            Func<int, bool> isSquareRoot = (x) => Math.Ceiling(Math.Sqrt(x)) == Math.Sqrt(x);

            var r =
                xs
                .Where(isSquareRoot)
                .Select(x => (int)Math.Sqrt(x));

            if (!r.Any())
                return new Tuple<IEnumerable<int>, int>(Enumerable.Empty<int>(), depth);

            return roots(r, depth + 1);
        }
    }

    [TestClass]
    public class Revise5Tests
    {
        [TestMethod]
        public void NumSquareRoots()
        {
            var n1 = new Revise5().NumSquareRoots(10,20);
            var n2 = new Revise5().NumSquareRoots(6000,7000);
            var n3 = new Revise5().NumSquareRoots(10,11);
            var n4 = new Revise5().NumSquareRoots(16,16);
            var n5 = new Revise5().NumSquareRoots(4,4);
            var invalid = new Revise5().NumSquareRoots(16,4);
            var big = new Revise5().NumSquareRoots(2,10000);

            Assert.AreEqual(2, n1);
            Assert.AreEqual(3, n2);
            Assert.AreEqual(0, n3);
            Assert.AreEqual(2, n4);
            Assert.AreEqual(1, n5);
            Assert.AreEqual(0, invalid);
            Assert.IsTrue(big >= 3);
        }
    }
}
