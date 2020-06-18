using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    // C# 6.0 .NET 4.5 (mono)
    class Codility01_BinaryGap
    {
        public class Accum { public List<int> list = new List<int>(); public int i = 0; }

        public int MonoGap(int n)
        {
            var indexes =
                Convert
                .ToString(n, 2)
                .ToCharArray()
                .Aggregate(
                    new Accum(),
                    (a, c) =>
                    {
                        if (c == '1')
                            a.list.Add(a.i);

                        a.i += 1;
                        return a;
                    },
                   a => a.list);

            if (indexes.Count <= 1)
                return 0;

            var gaps =
                indexes
                .Zip(indexes.Skip(1), (x, y) => Tuple.Create(x, y))
                .Select(a => a.Item2 - a.Item1);

            return gaps.Max() - 1;
        }

        public int Gap(int n)
        {
            var indexes = 
                Convert
                .ToString(n, 2)
                .ToCharArray()
                .Aggregate(
                    (list: new List<int>(), i: 0),
                    (a, c) =>
                    {
                        if (c == '1')
                            a.list.Add(a.i);

                        a.i += 1;
                        return a;
                    },
                   a => a.list);

            if (indexes.Count <= 1)
                return 0;

            var gaps =
                indexes
                .Zip(indexes.Skip(1), (x, y) => (x, y))
                .Select(a => a.y - a.x);

            return gaps.Max() - 1;
        }
    }

    [TestClass]
    public class Codility01_BinaryGapTests
    {
        Codility01_BinaryGap bg = new Codility01_BinaryGap();

        [TestMethod]
        public void Number1041_10000010001_HasGap_5()
        {
            Assert.AreEqual(5, bg.Gap(1041));
        }

        [TestMethod]
        public void Number529_1000010001_HasGap_4()
        {
            Assert.AreEqual(4, bg.Gap(529));
        }

        [TestMethod]
        public void Number20_10100_HasGap_1()
        {
            Assert.AreEqual(1, bg.Gap(20));
        }

        [TestMethod]
        public void Number15_1111_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(15));
        }

        [TestMethod]
        public void Number0_0_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(0));
        }

        [TestMethod]
        public void Number1_1_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(1));
        }

        [TestMethod]
        public void Number2_10_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(2));
        }

        [TestMethod]
        public void Number3_11_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(3));
        }

        [TestMethod]
        public void Number4_100_HasGap_0()
        {
            Assert.AreEqual(0, bg.Gap(4));
        }

        [TestMethod]
        public void Number5_101_HasGap_1()
        {
            Assert.AreEqual(1, bg.Gap(5));
        }

        [TestMethod]
        public void Number10_1010_HasGap_1()
        {
            Assert.AreEqual(1, bg.Gap(10));
        }

        [TestMethod]
        public void Number17_1001_HasGap_3()
        {
            Assert.AreEqual(3, bg.Gap(17));
        }
    }
}
