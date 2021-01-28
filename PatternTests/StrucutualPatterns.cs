using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrucutralPatterns.Adapter;
using StrucutralPatterns.Composite;
using StrucutralPatterns.Facade;

namespace PatternTests
{
    [TestClass]
    public class StrucutualPatterns
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
    }
}
