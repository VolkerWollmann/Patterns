using Patterns.Examples;
using Xunit;

namespace PatternTests
{
    public class PatternExampleTests
    {
        [Fact]
        public void StateExample()
        {
            Patterns.Examples.StateExample.Test();
        }

		[Theory]
		[InlineData("42", 42)]
		[InlineData("1 + 2", 2)]
		[InlineData("1 + 2 + 3", 3)]
		[InlineData("1 + (2 * 3)", 3)]
		public void Visitor_Simple(string expression, int expectedResult)
        {
            Assert.Equal(expectedResult, VisitorExample.SimpleVisitor(expression));
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
            Assert.Equal(expectedResult, VisitorExample.TransformingVisitor(expression));
        }

        [Fact]
        public void DependencyInjectionExample2()
        {
            DependencyInjectionExample2Test.Example();
        }
    }
}
