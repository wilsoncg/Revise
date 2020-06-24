using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviseApp
{
    public static class Utility
    {
        public static IEnumerable<int> Generate(int n)
        {
            var random = new Random();
            for (int i = 1; i < n; i++)
            {
                yield return random.Next(n);
            }
        }

        public static IEnumerable<char> GenerateGenomic(int n)
        {
            var random = new Random();
            var nucleotides = "AGCT";
            for (int i = 1; i < n; i++)
            {
                yield return nucleotides[random.Next(4)];
            }
        }
    }
}
