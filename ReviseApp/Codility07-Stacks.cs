using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviseApp
{
    public class Codility07_Stacks
    {
        public int ProperParenthesis(string s)
        {
            var parenthesis = new Stack<char>();
            const int notProperlyNested = 0;
            const int properlyNested = 1;

            foreach (var x in s.Select(c => c))
            {
                if (x == '(' || x == '{' || x == '[')
                {
                    parenthesis.Push(x);
                }                
                else if (parenthesis.Count != 0)
                {
                    var compare = parenthesis.Peek();
                    var openClose = compare == '(' && x == ')';
                    var openCloseCurly = compare == '{' && x == '}';
                    var openCloseSquare = compare == '[' && x == ']';
                    
                    if (openClose || 
                        openCloseCurly ||
                        openCloseSquare)
                    {
                        parenthesis.Pop();
                    }
                }                
            }

            var result =
                parenthesis.Count == 0 ?
                properlyNested :
                notProperlyNested;

            return result;
        }
    }

    [TestClass]
    public class Codility07_Stacks_Tests
    {
        Codility07_Stacks stacks = new Codility07_Stacks();

        [TestMethod]
        public void ProperParenthesis()
        {
            Assert.AreEqual(1, stacks.ProperParenthesis("{[()()]}"));
        }

        [TestMethod]
        public void InProperParenthesis()
        {
            Assert.AreEqual(0, stacks.ProperParenthesis("([)()]"));
        }
    }
}
