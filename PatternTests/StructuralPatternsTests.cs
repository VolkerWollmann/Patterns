using Patterns.Structural;
using Patterns.Structural.Decorator;
using Xunit;

namespace PatternTests
{
    public class StructuralPatternsTests
    {
        [Fact]
        public void Adapter()
        {
            AdapterExample.Adapter();
        }

        [Fact]
        public void Bridge()
        {
            BrigdeExample.Test();
        }

        [Fact]
        public void Composite()
        {
            CompositeExample.Composite();
        }

        [Fact]
        public void Decorator()
        {
            DecoratorExample.Decorator();
        }

        [Fact]
        public void Facade()
        {
            FacadeExample.Facade();
        }

        [Fact]
        public void Flyweight()
        {
            FlyweightExample.Test();
        }

        [Fact]
        public void Proxy()
        {
            Patterns.Structural.Proxy.ProxyExample.Test();
        }

	}
}
