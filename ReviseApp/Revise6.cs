using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fsharp_ReviseApp;

namespace ReviseApp
{
    public class Revise6
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

        public string ToFullShip(string s, int n)
        {
            var x0 = s[0];
            var y0 = s[1];
            var x1 = s[3];
            var y1 = s[4];

            var isRow = (x0 == x1) || (y0 == y1);
            if (isRow)
                return s;

            var row1 = RowToInt(x0);
            var row2 = RowToInt(x1);

            return $"{x0}{y0} {x0}{y1} {RowToInt(x0)+1}";
        }
    }

    [TestClass]
    public class BattleshipsTests
    {
        [TestMethod]
        public void Test()
        {
            var s1 = "1A 2B";
            var s2 = "1A 1B";
            var s3 = "2A 2B";
            var s4 = "1A 2A";

            Assert.AreEqual("1A 1B 2A 2B", Battleships.ToFullShip(s1, 2));
            Assert.AreEqual("1A 1B", Battleships.ToFullShip(s2, 2));
            Assert.AreEqual("2A 2B", Battleships.ToFullShip(s3, 2));
            Assert.AreEqual("1A 2A", Battleships.ToFullShip(s4, 2));
        }
    }
}
