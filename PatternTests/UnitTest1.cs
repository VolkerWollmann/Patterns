using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.VisitorExample;

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

        [TestMethod]
        public void TestSimpleVisitor()
        {
            VisitorExample.SimpleVisitor();
        }

        [TestMethod]
        public void TestTransformingVisitor()
        {
            VisitorExample.TransformingVisitor();
        }
    }
}
