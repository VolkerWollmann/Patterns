using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.Examples;

namespace PatternTests
{
    [TestClass]
    public class PatternExampleTests
    {
        [TestMethod]
        public void StateExample()
        {
            Patterns.Examples.StateExample.Test();
        }

        [TestMethod]
        public void Visitor_Simple()
        {
            VisitorExample.SimpleVisitor();
        }

		[DataTestMethod]
		[DataRow("42", 42)]
		[DataRow("1 + 2", 3)]
		[DataRow("1 + 2 + 3", 6)]
        [DataRow("1 + (2 * 3)", 7)]
        [DataRow("1 + 2 * 3", 7)]
        [DataRow("(3 + (4 * 5)) + (5 * (8 * 2))", 103)]
        [DataRow("43-1", 42)]
        public void Visitor_Transforming(string expression, int expectedResult)
        {
            VisitorExample.TransformingVisitor(expression,expectedResult);
        }
    }
}
