using Fsharp_ReviseApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ReviseApp
{
    [TestClass]
    public class FizzBuzzTests
    {
        [TestMethod]
        public void EmptyList()
        {
            var result = new FizzBuzzGenerator().Apply(Enumerable.Empty<int>()).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FirstResult()
        {
            var result = new FizzBuzzGenerator().Apply(Enumerable.Range(1, 1)).ToList();

            Assert.AreEqual("1", result.First());
        }

        [TestMethod]
        public void FirstFizz()
        {
            var result = new FizzBuzzGenerator().Apply(Enumerable.Range(1, 100)).ToList();

            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("Fizz", result[2]);
        }

        [TestMethod]
        public void More()
        {
            var result = new FizzBuzzGenerator().Apply(Enumerable.Range(1, 100)).ToList();

            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("Fizz", result[2]);
            Assert.AreEqual("4", result[3]);
            Assert.AreEqual("Buzz", result[4]);
            Assert.AreEqual("Fizz", result[5]);
            Assert.AreEqual("FizzBuzz", result[14]);
        }
    }
}
