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
                    if (countNucleobases[0, q] - countNucleobases[0, p] > 0)
                        return impactFactor(bases[0]);
                    if (countNucleobases[1, q] - countNucleobases[1, p] > 0)
                        return impactFactor(bases[1]);
                    if (countNucleobases[2, q] - countNucleobases[2, p] > 0)
                        return impactFactor(bases[2]);
                    if (countNucleobases[3, q] - countNucleobases[3, p] > 0)
                        return impactFactor(bases[3]);
                    
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
        public void GenomicRange()
        {
            var r1 = prefixSums.GenomicRange_Naive("CAGCCTA", new[] { 2, 5, 0 }, new[] { 4, 5, 6 });
            var r2 = prefixSums.GenomicRange_PreCompute("CAGCCTA", new[] { 2, 5, 0 }, new[] { 4, 5, 6 });
            CollectionAssert.AreEqual(new[] { 2, 4, 1 }, r1);
            CollectionAssert.AreEqual(new[] { 2, 4, 1 }, r2);
        }

        [TestMethod]
        public void GenomicRange_DoubleCharacterString()
        {
            var r1 = prefixSums.GenomicRange_Naive("AC", new[] { 0, 0, 1 }, new[] { 0, 1, 1 });
            CollectionAssert.AreEqual(new[] { 1, 1, 2 }, r1);

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

        [TestMethod]
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
