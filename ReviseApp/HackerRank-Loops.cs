using System;
using System.Linq;

namespace ReviseApp
{
    class HackerRank_Loops
    {
        // A single integer N
        // Constraints
        // > 2 <= N <= 20
        //
        // Output Format
        // Print 10 lines of output; each line i (where 1 <= i <= 10) contains the result of N x i in the form:
        // N x i = result.

        public void Run(int N)
        {
            var output =
                Enumerable
                .Range(1, 10)
                .Select(i => $"{N} x {i} = {i * N}");

            Console.Write(string.Join(Environment.NewLine, output));
        }
    }
}
