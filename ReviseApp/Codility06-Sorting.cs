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
    }
}
