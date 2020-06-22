using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
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
    }
}
