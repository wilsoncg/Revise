using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class Revise_QuickSort
    {
        public IEnumerable<int> Sort(IEnumerable<int> input)
        {
            // quicksort []     = []
            // quicksort (x:xs) = quicksort ys ++ [x] ++ quicksort xs
            //          where
            //          ys = [a | a <- xs, a <= x]
            //          zs = [b | b <- xs, b > x]

            if (!input.Any())
                return Enumerable.Empty<int>();

            var x = input.Take(1).First();
            var xs = input.Skip(1);

            var firstHalf = (xs.Count() / 2);
            //var secondHalf = (xs.Count() - (xs.Count() / 2));
            //var ys = xs.Take(firstHalf);
            //var zs = xs.Skip(firstHalf);

            var ys = Enumerable.Empty<int>();
            if(xs.Any() && xs.First() <= x)
                ys = ys.Concat(new[] { xs.First() });

            var zs = Enumerable.Empty<int>();
            if (xs.Any() && xs.First() > x)
                zs = zs.Concat(new[] { xs.First() });
            
            return Sort(ys).Concat((new[] { x }).ToList()).Concat(Sort(zs));
        }
    }

    [TestClass]
    public class Revise_QuickSortTests
    {
        [TestMethod]
        public void Test()
        {
            var input = new[] { 3, 6, 4, 1, 5, 2 };
            var sorted = new Revise_QuickSort().Sort(input).ToList();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, sorted);
        }
    }
}
