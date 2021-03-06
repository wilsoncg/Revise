﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var s = 
                scanForLowestY(toPointWithIndex(A))
                .Take(3)
                .ToList();
            var notInConvexHull = new List<int>();

            for (int i = 3; i < A.Count(); i++)
            {
                while (
                    s.Count() > 1 &&
                    AngleBetween3Points(s[s.Count() - 2].Point, s[s.Count() - 1].Point, A[i]) < 180)
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

        public IList<PointWithIndex> scanForLowestY(IList<PointWithIndex> points)
        {
            if (!points.Any() || points.Count == 1)
                return points;

            var first = points.First();
            var minY =
                points
                .Aggregate(
                    first,
                    (acc, x) => 
                    {
                        if (x.Point.y < acc.Point.y)
                            return x;
                        return acc;
                    });

            if (minY.index == first.index)
                return points;

            var left = points.Take(minY.index);
            var right = points.Skip(minY.index);
            return right.Concat(left).ToList();
        }

        IList<PointWithIndex> toPointWithIndex(Point2D[] points)
        {
            var list = new List<PointWithIndex>();
            points.Aggregate(
                0,
                (i, x) =>
                {
                    list.Add(new PointWithIndex { Point = x, index = i });
                    return i + 1;
                });
            return list;
        }               

        public double AngleBetween3Points(
            Point2D a, Point2D b, Point2D c)
        {
            var r = AngleBetween3Points(a.x, a.y, b.x, b.y, c.x, c.y);
            return r;
        }

        public double AngleBetween3Points(
            int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return angle(new Point2D(x1, y1), new Point2D(x2, y2), new Point2D(x3, y3));
        }

        double radiansToDegrees(double r)
        {
            return r * (180 / Math.PI);
        }

        double angle(Point2D p1, Point2D p2, Point2D p3)
        {
            var vector1 = new Vector(p2.x - p1.x, p2.y - p1.y);
            var vector2 = new Vector(p2.x - p3.x, p2.y - p3.y);
            var sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            var cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            var angle = Math.Atan2(sin, cos) * (180 / Math.PI);
            return Math.Abs(angle);
        }

        //double theAngle(Point2D p1, Point2D p2, Point2D p3)
        //{
        //    var a = new Vector2(p1.x - p2.x, p1.y - p2.y);
        //    var b = new Vector2(p2.x - p3.x, p2.y - p3.y);
        //    Func<Vector2, double> magnitude = v =>
        //    {
        //        return Math.Sqrt(v.X * v.X + v.Y * v.Y);
        //    };
        //    var radians = Math.Acos(Vector2.Dot(a, b) / (magnitude(a) * magnitude(b)));
        //    return radiansToDegrees(radians);
        //}

        //double Angle(
        //    int x1, int y1, int centrex, int centrey, int x2, int y2)
        //{
        //    var transformedP1x = (x1 - centrex);
        //    var transformedP1y = (y1 - centrey);
        //    var transformedP2x = (x2 - centrex);
        //    var transformedP2y = (y2 - centrey);

        //    var radians = 
        //        Math.Atan2(transformedP2y, transformedP2x) - 
        //        Math.Atan2(transformedP1y, transformedP1x);

        //    if (radians < 0)
        //    {
        //        var a = (radians + 2 * Math.PI);
        //        return radiansToDegrees(a > Math.PI ? (2 * Math.PI) - a : a); 
        //    }
        //    if (radians > Math.PI)
        //        return radiansToDegrees((2 * Math.PI) - radians);

        //    return radiansToDegrees(radians);
        //}

        public Point2D[] ToPoints(IEnumerable<(int x, int y)> points)
        {
            return 
                points
                .Select(p => new Point2D(p.x, p.y))
                .ToArray();
        }

        public class Vector { 
            public int X; public int Y;
            public Vector(int X, int Y) { this.X = X; this.Y = Y; }
        }
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
        public void AngleCollinear_NonNegativeXTest()
        {
            var r1 = pci.AngleBetween3Points(1, 2, 3, 1, 5, 0);
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

        [TestMethod]
        public void AngleLessThan20()
        {
            var r1 = pci.AngleBetween3Points(0, 1, 2, 0, 1, 1);
            Assert.IsTrue(r1 > 0 & r1 < 20, $"was {r1}");
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
                (-1, -2),
                (-2, -4),
                (-3, -6),
                (-3, 0),
                (-3, 1),
                (-3, 2),
                (-3, 3),
                (-3, 4)
            };

            var r1 = pci.PolygonConcavitiyIndex(pci.ToPoints(collinear));
            Assert.AreEqual(-1, r1);
        }

        [TestMethod]
        public void CheckStar()
        {
            var star = new[]
            {
                (0,1), // in hull
                (2,0), // on border
                (1,1), // in hull
                (2,2), // on border
                (1,2), // in hull
                (0,4), // on border
                (-1,2), // in hull
                (-2,2), // on border
                (-1,1), // in hull
                (-2,0) // on border
            };

            var r1 = pci.PolygonConcavitiyIndex(pci.ToPoints(star));
            var inhull = new[] { 0, 2, 4, 6, 8 };
            Assert.IsTrue(inhull.Contains(r1), $"Expected 0,2,4,6,8 but got {r1}");
        }

        [TestMethod]
        public void CheckHalfStar()
        {
            var star = new[]
            {
                (0,1), // on border
                (2,0), // on border
                (1,1), // in hull
                (2,2), // on border
                (1,2), // in hull
                (0,4) // on border
            };

            var r1 = pci.PolygonConcavitiyIndex(pci.ToPoints(star));
            var inhull = new[] { 2, 4 };
            Assert.IsTrue(inhull.Contains(r1), $"Expected 2,4 but got {r1}");
        }

        // Polygon has exactly one angle equals to (90 + epislon) degrees

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
