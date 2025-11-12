using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.Examples;
using Xunit;

namespace PatternTests
{
    [TestClass]
    public class PatternExampleTests
    {
        [Fact]
        public void StateExample()
        {
            Patterns.Examples.StateExample.Test();
        }

        [Fact]
        public void Visitor_Simple()
        {
            VisitorExample.SimpleVisitor();
        }

		[Theory]
		[InlineData("42", 42)]
		[InlineData("1 + 2", 3)]
		[InlineData("1 + 2 + 3", 6)]
        [InlineData("1 + (2 * 3)", 7)]
        [InlineData("1 + 2 * 3", 7)]
        [InlineData("(3 + (4 * 5)) + (5 * (8 * 2))", 103)]
        [InlineData("43-1", 42)]
        public void Visitor_Transforming(string expression, int expectedResult)
        {
            VisitorExample.TransformingVisitor(expression,expectedResult);
        }
    }
}
