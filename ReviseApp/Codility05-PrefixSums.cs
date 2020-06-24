using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Codility05_PrefixSums
    {
        public int CountDiv(int A, int B, int K)
        {
            // [A..B] divisible by K
            // the set { i : A ≤ i ≤ B, i mod K = 0 }

            var r = (B / K) - (A / K) + (A % K == 0 ? 1 : 0);
            return r;
        }
    }

    [TestClass]
    public class Codility05_PrefixSumsTests
    {
        Codility05_PrefixSums prefixSums = new Codility05_PrefixSums();

        [TestMethod]
        public void CountDiv()
        {
            Assert.AreEqual(3, prefixSums.CountDiv(6, 11, 2));
        }

        [TestMethod]
        public void CountDiv_minimal()
        {
            Assert.AreEqual(1, prefixSums.CountDiv(0, 0, 11));
            Assert.AreEqual(0, prefixSums.CountDiv(1, 1, 11));
        }

        [TestMethod]
        public void CountDiv_extreme()
        {
            Assert.AreEqual(1, prefixSums.CountDiv(10, 10, 5));
            Assert.AreEqual(0, prefixSums.CountDiv(10, 10, 7));
            Assert.AreEqual(0, prefixSums.CountDiv(10, 10, 20));
        }
    }
}
