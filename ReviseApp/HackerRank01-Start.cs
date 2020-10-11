using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReviseApp
{
    public class HackerRank01_Start
    {
        public int Main()
        {
            return 1;
        }

        public static string test(int n)
        {
            (int mod3, int mod5) = (n % 3, n % 5);
            if (mod3 == 0 && mod5 == 0)
                return "FizzBuzz";
            if (mod3 == 0)
                return "Fizz";
            if (mod5 == 0)
                return "Buzz";

            return n.ToString();

            // C# 8.0 - .net standard 2.0
            //var result = (n % 3, n % 5) switch
            //{
            //    (0, 0) => "FizzBuzz",
            //    (0, _) => "Fizz",
            //    (_, 0) => "Buzz",
            //    (_, _) => n.ToString()
            //};
            //return result;
        }
    }

    [TestClass]
    public class HackerRank01_StartTests
    {
        HackerRank01_Start start = new HackerRank01_Start();

        [TestMethod]
        public void Main_Test()
        {
            Assert.AreEqual(1, start.Main());
        }
    }
}
