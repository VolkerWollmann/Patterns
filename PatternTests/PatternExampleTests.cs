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

        [TestMethod]
        public void Visitor_Transforming()
        {
            VisitorExample.TransformingVisitor();
        }
    }
}
