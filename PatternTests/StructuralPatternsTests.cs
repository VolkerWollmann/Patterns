using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructuralPatterns.Adapter;
using StructuralPatterns.Composite;
using StructuralPatterns.Facade;
using StructuralPatterns.Iterator;

namespace PatternTests
{
    [TestClass]
    public class StructuralPatternsTests
    {
        [TestMethod]
        public void Adapter()
        {
            AdapterExample.Adapter();
        }

        [TestMethod]
        public void Composite()
        {
            CompositeExample.Composite();
        }

        [TestMethod]
        public void Facade()
        {
            FacadeExample.Facade();
        }

        [TestMethod]
        public void Iterator()
        {
            IteratorExample.Iterator();
        }
    }
}
