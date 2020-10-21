using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;

namespace PatternTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestVisitor()
        {
            // Visitor
            Vistor visitor = new Vistor();
            visitor.Main();
        }
    }
}
