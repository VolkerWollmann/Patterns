using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.Structural;
using Patterns.Structural.Decorator;

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
        public void Decorator()
        {
            DecoratorExample.Decorator();
        }

        [TestMethod]
        public void Facade()
        {
            FacadeExample.Facade();
        }

        [TestMethod]
        public void Flyweight()
        {
            FlyweightExample.Test();
        }

        [TestMethod]
        public void Proxy()
        {
            Patterns.Structural.Proxy.ProxyExample.Test();
        }

		[TestMethod]
		public void Serialisation()
		{
			Patterns.Structural.SerialisationExample.DoSerialisation();
		}
	}
}
