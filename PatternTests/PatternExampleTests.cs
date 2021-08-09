using System;
using BehaviourPatterns.VisitorExample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructuralPatterns.Adapter;
using StructuralPatterns.Composite;
using StructuralPatterns.Facade;
using StructuralPatterns.Iterator;

namespace PatternExampleTests
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
