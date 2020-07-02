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

            Assert.AreEqual(-5*-5*5, r1);
        }
    }
}
