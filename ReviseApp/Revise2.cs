using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise2
    {
        // million items, reasonable time, sorted ascending
        public bool Exists(int[] ints, int k)
        {
            // 7ms
            var s1 = Stopwatch.StartNew();
            var linq = ints.Any(x => x == k);
            s1.Stop();

            // 2ms
            var s2 = Stopwatch.StartNew();
            var bs = ints.ToList().BinarySearch(k) >= 0;
            s2.Stop();

            Console.WriteLine($"Linq took {s1.ElapsedMilliseconds} ms");
            Console.WriteLine($"BinarySearch took {s2.ElapsedMilliseconds} ms");
            return bs;
        }

        // https://app.codility.com/programmers/lessons/2-arrays/cyclic_rotation/
        public int[] CyclicRotation(int[] A, int K)
        {
            if (!A.Any())
                return new int[] { };

            var r = 
                (K > A.Length) ? 
                A.Length - (K % A.Length) : 
                (A.Length - K);

            var a = A.Skip(r);
            var result = a.Concat(A.Take(r)).ToArray();
            return result;
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
    }

    [TestClass]
    public class Revise2Tests
    {
        [TestMethod]
        public void MissingInteger_basic()
        {
            Assert.AreEqual(4, new Revise2().MissingInteger(new[] { 1, 2, 3 }));
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { -1, -3 }));
            Assert.AreEqual(5, new Revise2().MissingInteger(new[] { 1, 3, 6, 4, 1, 2 }));
        }

        [TestMethod]
        public void MissingInteger_extreme_single()
        {
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { 2 }));
        }

        [TestMethod]
        public void MissingInteger_failed()
        {
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { -5, -4, -3, -2, -1 }));
            Assert.AreEqual(1, new Revise2().MissingInteger(new[] { 0, 2, 3, 5, 6, 7 }));
        }

        [TestMethod]
        public void CyclicRotationTest()
        {
            var input1 = new[] { 3, 8, 9, 7, 6 };
            var input2 = new[] { 0, 0, 0 };
            var input3 = new[] { 1, 2, 3, 4 };
            var input4 = new[] { 1, 1, 2, 3, 5 };

            var r = new Revise2();

            CollectionAssert.AreEqual(new[] { 9, 7, 6, 3, 8}, r.CyclicRotation(input1, 3));
            CollectionAssert.AreEqual(new[] { 0, 0, 0 }, r.CyclicRotation(input2, 1));
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, r.CyclicRotation(input3, 4));
            CollectionAssert.AreEqual(new[] { 4, 1, 2, 3 }, r.CyclicRotation(input3, 1));
            CollectionAssert.AreEqual(new[] { 3, 4, 1, 2 }, r.CyclicRotation(input3, 2));
            CollectionAssert.AreEqual(new[] { 2, 3, 4, 1 }, r.CyclicRotation(input3, 3));
            CollectionAssert.AreEqual(new[] { 3, 5, 1, 1, 2 }, r.CyclicRotation(input4, 42));
            CollectionAssert.AreEqual(new int[] { }, r.CyclicRotation(new int[] { }, 1));
        }        

        [TestMethod]
        public void ExistsTest()
        {
            var million = Enumerable.Range(0, 1_000_000).ToArray();
            var r = new Revise2();

            Assert.IsTrue(r.Exists(million, 999_000));
            Assert.IsFalse(r.Exists(million, 1_000_000));
        }
    }
}
