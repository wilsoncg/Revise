using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Codility05_PrefixSums
    {
        public int PassingCars(int[] A)
        {
            int numCrossings = 0, index = 0, swapPoint = 0;
            foreach(var x in A)
            {
                if (numCrossings > 1_000_000_000)
                    return -1;

                if (x == 0)
                {
                }
                else
                {
                    // distance away from swapPoint?
                    var awayfrom = index - swapPoint;
                    numCrossings += awayfrom;
                    swapPoint++;
                }
                index++;
            }
            return numCrossings;
        }

        public int MinAvgTwoSlice(int[] A)
        {
            if (!A.Any())
                return 0;

            var minAvg = (A[0] + A[1]) / 2.0;
            var minAvgPos = 0;

            for(int index = 0; index < A.Length - 2; index++)
            {
                // Check next 2 element slice
                if((A[index] + A[index + 1])/2.0 < minAvg)
                {
                    minAvg = (A[index] + A[index + 1]) / 2.0;
                    minAvgPos = index;
                }
                // Check next 3 element slice
                if((A[index] + A[index + 1] + A[index + 2]) / 3.0 < minAvg)
                {
                    minAvg = (A[index] + A[index + 1] + A[index + 2]) / 3.0;
                    minAvgPos = index;
                }
                // Check last 2 element slice
                if ((A[A.Length - 1] + A[A.Length - 2]) / 2.0 < minAvg)
                {
                    minAvg = (A[A.Length - 1] + A[A.Length - 2]) / 2.0;
                    minAvgPos = A.Length - 2;
                }
            }            

            return minAvgPos;
        }
        
        public int CountDiv(int A, int B, int K)
        {
            // [A..B] divisible by K
            // the set { i : A ≤ i ≤ B, i mod K = 0 }

            var r = (B / K) - (A / K) + (A % K == 0 ? 1 : 0);
            return r;
        }

        int impactFactor(char c)
        {
            switch (c)
            {
                case 'A':
                    return 1;
                case 'C':
                    return 2;
                case 'G':
                    return 3;
                case 'T':
                    return 4;
            }
            return 0;
        }

        public class GenomeAccum {
            public int SequenceIndex { get; set; }
            public int CountA { get; set; } 
            public int CountC { get; set; }
            public int CountG { get; set; }
            public int CountT { get; set; }
        }

        public int[] GenomicRange_PreCompute(string s, int[] P, int[] Q)
        {
            var bases = "ACGT";
            var dna = s;
            var countNucleobases = new int[bases.Length, dna.Length];

            // total up number of times each base appears in dna sequence
            dna.Aggregate(
                    new GenomeAccum(),
                    (accum, c) => {
                        if (c == 'A')
                        {
                            accum.CountA++;
                        }
                        else if(c == 'C')
                        {
                            accum.CountC++;
                        }
                        else if (c == 'G')
                        {
                            accum.CountG++;
                        }
                        else if (c == 'T')
                        {
                            accum.CountT++;
                        }

                        countNucleobases[0, accum.SequenceIndex] = accum.CountA;
                        countNucleobases[1, accum.SequenceIndex] = accum.CountC;
                        countNucleobases[2, accum.SequenceIndex] = accum.CountG;
                        countNucleobases[3, accum.SequenceIndex] = accum.CountT;

                        accum.SequenceIndex++;
                        return accum;
                    });

            var queries = P.Zip(Q, (p, q) => Tuple.Create(p, q));
            var answers =
                queries
                .Select(x => {
                    var p = x.Item1;
                    var q = x.Item2;

                    if (p == q)
                        return impactFactor(dna[p]);

                    for(var bIndex = 0; bIndex < bases.Length; bIndex++)
                    {
                        var countAtP = countNucleobases[bIndex, p];
                        var countAtQ = countNucleobases[bIndex, q];
                        
                        if (p == 0 && countAtQ > 0)
                            return impactFactor(bases[bIndex]);

                        if (countAtQ - countAtP > 0 || 
                            (p > 0 && countNucleobases[bIndex, p - 1] < countAtP))
                        {
                            return impactFactor(bases[bIndex]);
                        }
                    }
                    
                    return 0;
                })
                .ToArray();
            return answers;
        }

        // O(N*M)
        public int[] GenomicRange_Naive(string s, int[] P, int[] Q)
        {
            var queries = P.Zip(Q, (p, q) => Tuple.Create(p, q));
            var answers =
                queries
                .Select(t => 
                {
                    var impact = 0;
                    for(int i=t.Item1; i <= t.Item2; i++)
                    {
                        var factor = impactFactor(s[i]);

                        if (impact == 0 || factor < impact)
                        {
                            impact = factor;
                        }
                    }
                    return impact;
                })
                .ToArray();
            return answers;
        }
    }

    [TestClass]
    public class Codility05_PrefixSumsTests
    {
        Codility05_PrefixSums prefixSums = new Codility05_PrefixSums();

        [TestMethod]
        public void PassingCars_Sample()
        {
            var r = prefixSums.PassingCars(new[] { 0, 1, 0, 1, 1 });
            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void PassingCars_AllEast()
        {
            var r = prefixSums.PassingCars(new[] { 0, 0, 0 });
            Assert.AreEqual(0, r);
        }

        [TestMethod]
        public void PassingCars_2East_1West()
        {
            var r = prefixSums.PassingCars(new[] { 0, 0, 1 });
            Assert.AreEqual(2, r);
        }

        [TestMethod]
        public void PassingCars_1East_1West()
        {
            var r = prefixSums.PassingCars(new[] { 0, 1, 0 });
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void PassingCars_1East_2West()
        {
            var r = prefixSums.PassingCars(new[] { 0, 1, 1 });
            Assert.AreEqual(2, r);
        }

        [TestMethod]
        public void PassingCars_1East_3West()
        {
            var r = prefixSums.PassingCars(new[] { 0, 1, 1, 1 });
            Assert.AreEqual(3, r);
        }

        [TestMethod]
        public void PassingCars_2East_2West_Staggered()
        {
            var r = prefixSums.PassingCars(new[] { 0, 1, 0, 1 });
            Assert.AreEqual(3, r);
        }

        [TestMethod]
        public void PassingCars_WestFirst()
        {
            var r1 = prefixSums.PassingCars(new[] { 1, 0, 0 });
            var r2 = prefixSums.PassingCars(new[] { 1, 1, 0 });
            var r3 = prefixSums.PassingCars(new[] { 1, 1, 1 });
            var r4 = prefixSums.PassingCars(new[] { 1, 0, 1 });
            Assert.AreEqual(0, r1);
            Assert.AreEqual(0, r2);
            Assert.AreEqual(0, r3);
            Assert.AreEqual(1, r4);
        }

        [TestMethod]
        public void AvgTwoSlice()
        {
            var r = prefixSums.MinAvgTwoSlice(new[] { 4, 2, 2, 5, 1, 5, 8 });
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void GenomicRange_Naive()
        {
            var r1 = prefixSums.GenomicRange_Naive(
                "CAGCCTA", 
                new[] { 2, 5, 0 }, 
                new[] { 4, 5, 6 });
            CollectionAssert.AreEqual(new[] { 2, 4, 1 }, r1);
        }

        [TestMethod]
        public void GenomicRange_PreCompute()
        {
            var r2 = prefixSums.GenomicRange_PreCompute(
                "CAGCCTA",
                new[] { 2, 5, 0 },
                new[] { 4, 5, 6 });
            CollectionAssert.AreEqual(new[] { 2, 4, 1 }, r2, 
                $"Expected [2,4,1] got [{string.Join(",", r2)}]");
        }

        [TestMethod]
        public void GenomicRange_PreCompute_SimpleTests()
        {
            var r1 = prefixSums.GenomicRange_PreCompute(
                "C", new[] { 0, 0, 0 }, new[] { 0, 0, 0 });
            var r2 = prefixSums.GenomicRange_PreCompute(
                "AA", new[] { 0, 1, 0 }, new[] { 0, 1, 1 });
            var r3 = prefixSums.GenomicRange_PreCompute(
                "CC", new[] { 0, 1, 0 }, new[] { 0, 1, 1 });
            var r4 = prefixSums.GenomicRange_PreCompute(
                "GG", new[] { 0, 1, 0 }, new[] { 0, 1, 1 });
            var r5 = prefixSums.GenomicRange_PreCompute(
                "TT", new[] { 0, 0, 0 }, new[] { 1, 1, 1 });
            var r6 = prefixSums.GenomicRange_PreCompute(
                "ATT", new[] { 0, 0, 0 }, new[] { 1, 1, 1 });
            var r7 = prefixSums.GenomicRange_PreCompute(
                "CAGTCAT", new[] { 0, 1, 3 }, new[] { 0, 5, 4 });

            check(r1, new[] { 2, 2, 2 });
            check(r2, new[] { 1, 1, 1 });
            check(r3, new[] { 2, 2, 2 });
            check(r4, new[] { 3, 3, 3 });
            check(r5, new[] { 4, 4, 4 });
            check(r6, new[] { 1, 1, 1 });
            check(r7, new[] { 2, 1, 2 });
        }

        void check(int[] r, int[] er)
        {
            CollectionAssert.AreEqual(r, er,
                    $"Expected [{string.Join(",", er)}] got [{string.Join(",", r)}]");
        }

        [TestMethod]
        public void GenomicRange_Comparison()
        {
            var seq = "TCAGCCT";
            var p = new[] { 2, 0, 1, 2 };
            var q = new[] { 4, 0, 3, 2 };
            var r1 = prefixSums.GenomicRange_Naive(seq, p ,q);
            var r2 = prefixSums.GenomicRange_PreCompute(seq, p, q);

            check(r2, r1);
        }

        [TestMethod]
        public void GenomicRange_DoubleCharacterString_Naive()
        {
            var r1 = prefixSums.GenomicRange_Naive("AC", new[] { 0, 0, 1 }, new[] { 0, 1, 1 });
            CollectionAssert.AreEqual(new[] { 1, 1, 2 }, r1);
        }

        [TestMethod]
        public void GenomicRange_DoubleCharacterString_PreCompute()
        {
            var r2 = prefixSums.GenomicRange_PreCompute("AC", new[] { 0, 0, 1 }, new[] { 0, 1, 1 });
            CollectionAssert.AreEqual(new[] { 1, 1, 2 }, r2);
        }

        double TimeGenomicRange(Func<string, int[], int[], int[]> f)
        {
            var N = 100_000;
            var dna =
                Utility
                .GenerateGenomic(N)
                .Aggregate(
                    new StringBuilder(),
                    (sb, c) =>
                    sb.Append(c),
                    sb => sb.ToString());
            var q = Utility.Generate(N - 1).ToArray();
            var m = N / 3;
            var p = q.Select(x => {
                return (x - m) > 0 ? (x - 5) : 0;
            }).ToArray();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = f(dna, p, q);
            stopwatch.Stop();

            return stopwatch.Elapsed.TotalSeconds;
        }

        [TestMethod, Ignore]
        public void GenomicRange_ForLargeRandom_NaiveIsGreaterThan6Seconds()
        {
            var timeInSeconds = TimeGenomicRange(prefixSums.GenomicRange_Naive);
            Assert.IsTrue(timeInSeconds > 6);
        }

        [TestMethod]
        public void GenomicRange_ForLargeRandom_PreComputeIsLessThan1Second()
        {
            var timeInSeconds = TimeGenomicRange(prefixSums.GenomicRange_PreCompute);
            Assert.IsTrue(timeInSeconds < 0.2);
        }

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
