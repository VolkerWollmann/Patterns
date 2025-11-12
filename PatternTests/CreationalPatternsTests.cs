using Patterns.CreationalPatterns;
using PCE = Patterns.CreationalPatterns.ExtensionMethodVsConstructor;
using Xunit;

namespace PatternTests
{
    public class CreationalPatternsTests
    {
        [Fact]
        public void AbstractFactory()
        {
            AbstractFactoryExample.AbstractFactory();
        }

        [Fact]
        public void FactoryMethod()
        {
            FactoryMethodExample.Test();
        }

        [Fact]
        public void ExtensionMethodVsConstructor()
        {
            PCE.ExtensionMethodVsConstructorExample.Example();
        }

        [Fact]
        public void Singleton()
        {
            SingletonExample.Example();
        }

        [Fact]
        public void Builder()
        {
            BuilderExample.Example();
        }

        [Fact]
        public void Prototype()
        {
            PrototypeExample.Test();
        }
    }
}
