using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviseApp
{
    // https://app.codility.com/programmers/lessons/99-future_training/polygon_concavity_index/
    // codility is c# 7 (mono)
    public class Codility99_PolygonConcavityIndex
    {
        IList<int> hull(Point2D[] A)
        {
            var s = toPointWithIndex(A).Take(3).ToList();
            var notInConvexHull = new List<int>();

            for (int i = 3; i < A.Count(); i++)
            {
                while (
                    s.Count() > 1 &&
                    AngleBetween3Points(
                        s[s.Count() - 2].Point,
                        s[s.Count() - 1].Point,
                        A[i]) < 180)
                {
                    notInConvexHull.Add(s[s.Count() - 1].index);
                    s.RemoveAt(s.Count() - 1);                    
                }
                s.Add(new PointWithIndex { Point = A[i], index = i });

                // compare last point
                if(i+1 == A.Count())
                {
                    var angle = AngleBetween3Points(
                        s[s.Count() - 2].Point,
                        s[s.Count() - 1].Point,
                        A[0]);
                    if(angle < 180)
                        notInConvexHull.Add(s[s.Count() - 1].index);
                }
            }

            return notInConvexHull;
        }

        IList<PointWithIndex> toPointWithIndex(Point2D[] points)
        {
            var list = new List<PointWithIndex>();
            points.Aggregate(
                0,
                (s, x) =>
                {
                    list.Add(new PointWithIndex { Point = x, index = s });
                    return s + 1;
                });
            return list;
        }               

        public double AngleBetween3Points(
            Point2D a, Point2D b, Point2D c)
        {
            var r = AngleBetween3Points(a.x, a.y, b.x, b.y, c.x, c.y);
            //Console.WriteLine($"Angle is {r} for {a.x},{a.y} {b.x},{b.y} {c.x},{c.y}");
            return r;
        }

        public double AngleBetween3Points(
            int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var v1 = new Vector() { X = x2 - x1, Y = y2 - y1 };
            var v2 = new Vector() { X = x3 - x2, Y = y3 - y2 };
            return Math.Abs(Angle(v1, v2));
        }

        double radiansToDegrees(double r)
        {
            return r * (180 / Math.PI);
        }

        double Angle(
            Vector v1, 
            Vector v2)
        {
            var radians = Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X);
            if (radians == 0)
                return radiansToDegrees(Math.PI);
            if (radians < 0)
                return radiansToDegrees(radians + (2 * Math.PI)); 

            return radiansToDegrees(radians);
        }

        public Point2D[] ToPoints(IEnumerable<(int x, int y)> points)
        {
            return 
                points
                .Select(p => new Point2D(p.x, p.y))
                .ToArray();
        }

        public class Vector { public int X; public int Y; }
        public class Point2D {
            public int x; public int y;
            public Point2D(int x, int y) { this.x = x; this.y = y; } 
        }
        public class PointWithIndex { public Point2D Point; public int index; }

        public int PolygonConcavitiyIndex(Point2D[] A)
        {
            if (A.Count() < 4)
                return -1;

            var notInHull = hull(A);
            if (!notInHull.Any())
                return -1;

            return notInHull.First();
        }
    }

    public class Util
    {
        public double LeftOnOrRight((int x,int y) p0, (int x, int y) p1, (int x, int y) p2)
        {
            return
                ((p1.x - p0.x) * (p2.y - p0.y)) -
                ((p2.x - p0.x) * (p1.y - p0.y));
        }

        public double LeftOnOrRight(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return LeftOnOrRight(
                (x1, y1),
                (x2, y2),
                (x3, y3));
        }
    }

    [TestClass]
    public class Codility99_PolygonConcavityIndexTests
    {
        private Codility99_PolygonConcavityIndex pci = new Codility99_PolygonConcavityIndex();
        private Util u = new Util(); 

        [TestMethod]
        public void IsConvexReturnsMinus1()
        {
            var input = new[] { (-1, 3), (1, 2), (3, 1), (0, -1), (-2, 1) };

            var result = pci.PolygonConcavitiyIndex(pci.ToPoints(input));

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void NotInHullReturnsEither2or6()
        {
            var input = new[] { (-1, 3), (1, 2), (1, 1), (3, 1), (0, -1), (-2, 1), (-1, 2) };

            var result = pci.PolygonConcavitiyIndex(pci.ToPoints(input));

            Assert.IsTrue(result == 2 | result == 6);
        }

        [TestMethod]
        public void LastPointNotInHull()
        {
            var input = new[] { (-1, 3), (1, 2), (3, 1), (0, -1), (-2, 1), (-1, 2) };

            var result = pci.PolygonConcavitiyIndex(pci.ToPoints(input));

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void SquareShouldBeMinus1()
        {
            var input = new[] { (1, 3), (3, 3), (3, 1), (1, 1) };

            var r3 = pci;
            var result = r3.PolygonConcavitiyIndex(r3.ToPoints(input));

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CollinearTest()
        {
            var r1 = u.LeftOnOrRight(-1, 3, 1, 2, 3, 1);
            Assert.AreEqual(0, r1);
        }

        [TestMethod]
        public void TurnLeft()
        {
            var r2 = u.LeftOnOrRight(1, 2, 1, 1, 3, 1);
            Assert.IsTrue(r2 > 0, $"was {r2}");
        }

        [TestMethod]
        public void TurnRight()
        {
            var r1 = u.LeftOnOrRight(1, 2, 3, 1, 0, -1);
            Assert.IsTrue(r1 < 0, $"was {r1}");
        }

        [TestMethod]
        public void TurnRightAlso()
        {
            var r1 = u.LeftOnOrRight(3, 1, 0, -1, -2, 1);
            Assert.IsTrue(r1 < 0, $"was {r1}");
        }

        [TestMethod]
        public void AngleCollinearTest()
        {
            var r1 = pci.AngleBetween3Points(-1, 3, 1, 2, 3, 1);
            Assert.AreEqual(180, r1);
        }

        [TestMethod]
        public void Angle90()
        {
            var r2 = pci.AngleBetween3Points(1, 2, 1, 1, 3, 1);
            Assert.IsTrue(r2 == 90, $"was {r2}");
        }

        [TestMethod]
        public void AngleGreaterThan90()
        {
            var r1 = pci.AngleBetween3Points(1, 2, 3, 1, 0, -1);
            Assert.IsTrue(r1 > 90, $"was {r1}");
        }

        [TestMethod]
        public void AnotherAngleGreaterThan90()
        {
            var r1 = pci.AngleBetween3Points(3, 1, 0, -1, -2, 1);
            Assert.IsTrue(r1 > 90, $"was {r1}");
        }

        // Boomerang
        [TestMethod]
        public void CheckBoomerang()
        {
            var boomerang = new[] { 
                (-1, 6), 
                (1, 7), 
                (3, 6), 
                (2, 4),
                (1, 2),
                (0, 0),
                (0, 2),
                (1, 3),
                (2, 5),
                (1, 6),
                (-1, 6)
            };

            var r1 = pci.PolygonConcavitiyIndex(pci.ToPoints(boomerang));
            Assert.AreEqual(7, r1);
        }

        [TestMethod]
        public void CheckManyCollinear()
        {
            var collinear = new[] {
                (-1, 6),
                (1, 7),
                (3, 6),
                (2, 4),
                (1, 2),
                (0, 0),
                (-1, -1),
                (-2, -2),
                (-1, 6)
            };

            var r1 = pci.PolygonConcavitiyIndex(pci.ToPoints(collinear));
            Assert.AreEqual(-1, r1);
        }

        // Star
        // Polygon has exactly one angle equals to (90 + epislon) degrees
        // collinear vertices

        /* 
        Unhandled Exception:
        System.ArgumentOutOfRangeException: Index was out of range.Must be non-negative and less than the size of the collection.
        Parameter name: index
          at System.ThrowHelper.ThrowArgumentOutOfRangeException ()[0x00000] in <filename unknown>:0 
          at System.Collections.Generic.List`1[Solution+PointWithIndex].get_Item (Int32 index)[0x00000] in <filename unknown>:0 
          at Solution.hull (Point2D[] A)[0x00000] in <filename unknown>:0 
          at Solution.solution (Point2D[] A)[0x00000] in <filename unknown>:0 
          at SolutionWrapper.run (System.String input, System.String output)[0x00000] in <filename unknown>:0 
          at SolutionWrapper.Main (System.String[] args)[0x00000] in <filename unknown>:0 
        */
    }
}
