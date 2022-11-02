using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.Structural;

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
        public void Bridge()
        {
            BrigdeExample.Test();
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

        [TestMethod]
        public void Proxy()
        {
            Patterns.Structural.Proxy.ProxyExample.Test();
        }
    }
}
