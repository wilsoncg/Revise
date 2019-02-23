using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ReviseApp
{
    public class Revise3
    {
        // https://app.codility.com/programmers/lessons/99-future_training/polygon_concavity_index/
        public int PolygonConcavitiyIndex(Point2D[] A)
        {
            var ordered = A.OrderByDescending(p => p.x);
            var pivot = ordered.First();

            var seed = new ConcavityAccum {
                NotInConvexHull = new List<int>(),
                Point = pivot };

            var r =
                ordered
                .Skip(1)
                .Aggregate(
                    seed,
                    (a, p) => 
                    {
                        //if(Math. a.Point)
                        return a;
                    },
                    (a) => a.NotInConvexHull);
            if (!r.Any())
                return -1;

            return r.First();
        }

        public double AngleBetween2Vectors(
            int x1, int y1,
            int x2, int y2)
        {
            return Angle(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public double AngleBetween3Points(
            int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return 
                Math.Abs(
                    Angle(
                        new Vector2(y3 - y1, x3 - x1),
                        new Vector2(y2 - y1, x2 - x1)));
        }

        double Angle(
            Vector2 v1, 
            Vector2 v2)
        {
            var radians = Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X);
            return radians * (180 / Math.PI);
        }

        class ConcavityAccum
        {
            public List<int> NotInConvexHull { get; set; }
            public Point2D Point { get; set; }
        }

        public Point2D[] ToPoints(IEnumerable<(int x, int y)> points)
        {
            return 
                points
                .Select(p => new Point2D { x = p.x, y = p.y })
                .ToArray();
        }
    }

    public class Point2D { public int x; public int y; }

    [TestClass]
    public class Revise3Tests
    {
        [TestMethod]
        public void IsConvexReturnsMinus1()
        {
            var input = new[] { (-1, 3), (1, 2), (3, 1), (0, -1), (-2, 1) };

            var r3 = new Revise3();
            var result = r3.PolygonConcavitiyIndex(r3.ToPoints(input));

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SquareShouldBeMinus1()
        {
            var input = new[] { (1, 3), (1, 1), (3, 1), (3, 3) };

            var r3 = new Revise3();
            var result = r3.PolygonConcavitiyIndex(r3.ToPoints(input));

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void Angle1()
        {
            var r1 = new Revise3().AngleBetween2Vectors(2, 0, 0, 1);
            var r2 = new Revise3().AngleBetween3Points(1, 1, 1, 2, 3, 1);
            Assert.AreEqual(90, r1);
            Assert.AreEqual(r1, r2);
        }
    }
}
