using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviseApp
{
    class Codility02_OddOccurences
    {
        public int Occurences(int[] A)
        {
            var r = 
                A.Aggregate(
                    new Dictionary<int, bool>(), 
                    (acc, x) =>
                    {
                        return Check(acc, x);
                    });

            return r.Any() ? r.Keys.First() : 0 ;
        }

        static Dictionary<int, bool> Check(
            Dictionary<int, bool> s, 
            int a)
        {
            if (s.ContainsKey(a))
            {
                s.Remove(a);
            }
            else
                s.Add(a, true);

            return s;
        }
    }

    [TestClass]
    public class Codility02_OddOccurencesTests
    {
        [TestMethod]
        public void SampleTest()
        {
            var input = new[] { 9, 3, 9, 3, 9, 7, 9 };
            var r = new Codility02_OddOccurences().Occurences(input);
            Assert.AreEqual(7, r);
        }
    }
}
