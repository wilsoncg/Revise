using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ReviseApp
{
    public class Battleships
    {
        int RowToInt(char c)
        {
            return int.Parse(c.ToString());
        }

        int ColumnToInt(char c)
        {
            return (int)c - 65;
        }

        string IntToColumn(int i)
        {
            var e = Enumerable.Range('A', 26).ElementAt(i);
            return ((char)e).ToString();
        }

        public string ToFullShip(string s)
        {
            var split = s.Split(' ');
            Func<string[], int, int> RowFromPin = (pins,index) => 
            {
                var numberAsString =
                    split
                    .ElementAt(index)
                    .Take(split.ElementAt(index).Length - 1)
                    .Select(x => x.ToString())
                    .Aggregate((x,y) => x + y);
                return int.Parse(numberAsString);
            };

            // 1A 1B == x0y0 x1y1
            var x0 = RowFromPin(split, 0);
            var y0 = split.ElementAt(0).Last();
            var x1 = RowFromPin(split, 1);
            var y1 = split.ElementAt(1).Last();

            var isRow = (x0 == x1);
            var isColumn = (y0 == y1);
            var isSingleSquare = (x0 == x1 && y0 == y1);
            if (isRow || isSingleSquare)
            {
                return s;
            }
            if (isColumn)
            {
                var c = x1 - x0;
                var rows = Enumerable.Range(x0, c + 1);
                var columns = new[] { s[1].ToString() };
                var r = rows.SelectMany(row => columns, (row, column) => row + column);
                return String.Join(" ", r);
            }

            return $"{x0}{y0} {x0}{y1} {x0 + 1}{y0} {x0 + 1}{y1}";
        }

        public (int,int) Play(string shipCoords, string hitList)
        {
            var counts = 
                shipCoords
                .Split(',')
                .Select(x => ToFullShip(x).HitsOrSunk(hitList));
            return counts.Aggregate((x, y) => { return (x.Item1 + y.Item1, x.Item2 + y.Item2); });
        }
    }

    public static class ShipExt
    {
        public static (int, int) HitsOrSunk(this string ship, string hitList)
        {
            var fullShip = ship.Split(' ');
            var hitCount = Hits(ship, hitList);
            var sunk = (fullShip.Count() == hitCount) ? 1 : 0;
            var hitNotSunk = (hitCount > 0) & (hitCount < fullShip.Count()) ? 1 : 0;

            return (sunk, hitNotSunk);
        }

        public static int Hits(this string ship, string hitList)
        {
            var fullShip = ship.Split(' ');
            var hits = fullShip.Count(x => hitList.Split(' ').Contains(x));            
            return hits;
        }
    }

    [TestClass]
    public class BattleshipsTests
    {
        string s1 = "1A 2B";
        string s2 = "1A 1B";
        string s3 = "2A 2B";
        string s4 = "1A 2A";
        string s5 = "2D 4D";

        [TestMethod]
        public void ToFullShipTest()
        {
            Assert.AreEqual("1A 1B 2A 2B", new Battleships().ToFullShip(s1));
            Assert.AreEqual("1A 1B", new Battleships().ToFullShip(s2));
            Assert.AreEqual("2A 2B", new Battleships().ToFullShip(s3));
            Assert.AreEqual("1A 2A", new Battleships().ToFullShip(s4));
            Assert.AreEqual("2D 3D 4D", new Battleships().ToFullShip(s5));
            Assert.AreEqual("2C 2C", new Battleships().ToFullShip("2C 2C"));
            Assert.AreEqual("12A 12A", new Battleships().ToFullShip("12A 12A"));
        }

        [TestMethod]
        public void HitsTest()
        {
            Assert.AreEqual(0, new Battleships().ToFullShip(s1).Hits(""));
            Assert.AreEqual(0, new Battleships().ToFullShip(s1).Hits(" "));
            Assert.AreEqual(0, new Battleships().ToFullShip(s4).Hits("3A"));
            Assert.AreEqual(0, new Battleships().ToFullShip(s4).Hits("1B"));

            Assert.AreEqual(1, new Battleships().ToFullShip(s1).Hits("1A"));
            Assert.AreEqual(1, new Battleships().ToFullShip(s1).Hits("1B"));
            Assert.AreEqual(1, new Battleships().ToFullShip(s1).Hits("2A"));
            Assert.AreEqual(1, new Battleships().ToFullShip(s1).Hits("2B"));

            Assert.AreEqual(2, new Battleships().ToFullShip(s1).Hits("1A 1B"));
            Assert.AreEqual(2, new Battleships().ToFullShip(s1).Hits("1A 2A"));
            Assert.AreEqual(2, new Battleships().ToFullShip(s1).Hits("1B 2A"));
            Assert.AreEqual(2, new Battleships().ToFullShip(s1).Hits("2B 2A"));

            Assert.AreEqual(3, new Battleships().ToFullShip(s1).Hits("1A 2A 2B"));
            Assert.AreEqual(4, new Battleships().ToFullShip(s1).Hits("1A 1B 2A 2B"));

            Assert.AreEqual((0,1), new Battleships().ToFullShip("1A 1B").HitsOrSunk("1B"));
            Assert.AreEqual((0,0), new Battleships().ToFullShip("2C 2C").HitsOrSunk("1B"));
            Assert.AreEqual((0,0), new Battleships().ToFullShip("1A 2A").HitsOrSunk("12A"));
            Assert.AreEqual((1,0), new Battleships().ToFullShip("12A 12A").HitsOrSunk("12A"));            
        }

        [TestMethod]
        public void PlayTest()
        {
            Assert.AreEqual((1, 0), new Battleships().Play("1A 2A,12A 12A", "12A"));
            Assert.AreEqual((0, 1), new Battleships().Play("1A 1B,2C 2C", "1B"));
            Assert.AreEqual((1, 1), new Battleships().Play("1B 2C,2D 4D", "2B 2D 3D 4D 4A"));
        }
    }
}
