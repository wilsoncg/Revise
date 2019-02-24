using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviseApp
{
    public class Revise4
    {
        public string solution(int T, int N, int[] V)
        {
            var rows = N;
            var columns = N;
            var scores = new List<Score>();
            var matrix = toMatrix(V, N);
            
            for (int row = 0; row < N; row++)
            {
                for (int column = 0; column < N; column++)
                {
                    if (row == 0 || column == 0 || row == N - 1 || column == N - 1)
                    {
                        Func<int, int> clip = x => { return x + 1 >= N ? x - 1 : x; };
                        var sum = sumNeareastAtEdge(matrix, clip(row), clip(column));
                        scores.Add(new Score { Location = (column, row), Sum = sum });
                    }
                    else
                    {
                        var sum = sumNearest(matrix, row, column);
                        scores.Add(new Score { Location = (column, row), Sum = sum });
                    }
                }
            }

            var s = 
                scores
                .OrderByDescending(x => x.Sum)
                .Take(T)
                .Select(x => $"({x.Location.x},{x.Location.y},{x.Sum})");
            return string.Join("", s);
        }

        int sumNeareastAtEdge(int[,] m, int i, int j)
        {
            return
                m[i, j] + m[i + 1, j] + m[i, j + 1] + m[i + 1, j + 1]; 
        }

        int sumNearest(int[,] m, int i, int j)
        {
            return
                // j -1
                m[i - 1, j - 1] +
                m[i, j - 1] +
                m[i + 1, j - 1] +
                // j
                m[i - 1, j] +
                m[i, j] +
                m[i + 1, j] +
                // j + 1
                m[i - 1, j + 1] +
                m[i, j + 1] +
                m[i + 1, j + 1];
        }
        
        public class Score { public (int x, int y) Location; public int Sum; }

        public int[,] toMatrix(int[] V, int N)
        {
            int[,] result = new int[N,N];
            for(int i = 0; i < N; i++)
            {
                for(int j = 0; j < N; j++)
                {
                    result[i,j] = V[i * N + j];
                }
            }
            return result;
        }
    }

    [TestClass]
    public class Revise4Tests
    {
        [TestMethod]
        public void Test1()
        {
            var result = new Revise4().solution(1, 2, new[] {4, 2, 0, 1});

            Assert.AreEqual("(0,0,7)", result);
        }

        [TestMethod]
        public void Test2()
        {
            var result = new Revise4().solution(1, 3, new[] { 4, 2, 3, 0, 1, 2, 1, 3, 0 });

            Assert.AreEqual("(1,1,16)", result);
        }

        [TestMethod]
        public void Test3()
        {
            var result = new Revise4().solution(1, 4, 
                new[] {
                    4, 2, 3, 2,
                    0, 1, 2, 2,
                    1, 3, 0, 2,
                    2, 0, 1, 5,
                });

            Assert.AreEqual("(2,1,17)", result);
        }

        [TestMethod]
        public void toMatrixTest()
        {
            var result = new Revise4().toMatrix(new[] { 4, 2, 0, 1 }, 2);

            Assert.AreEqual(4, result[0,0]);
            Assert.AreEqual(2, result[0,1]);
            Assert.AreEqual(0, result[1,0]);
            Assert.AreEqual(1, result[1,1]);
        }
    }
}
