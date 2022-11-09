using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.CreationalPatterns;
using PCE = Patterns.CreationalPatterns.ExtensionMethodVsConstructor;

namespace PatternTests
{
    [TestClass]
    public class CreationalPatternsTests
    {
        [TestMethod]
        public void AbstractFactory()
        {
            AbstractFactoryExample.AbstractFactory();
        }

        [TestMethod]
        public void FactoryMethod()
        {
            FactoryMethodExample.Test();
        }

        [TestMethod]
        public void ExtensionMethodVsConstructor()
        {
            PCE.ExtensionMethodVsConstructorExample.Example();
        }

        [TestMethod]
        public void Singleton()
        {
            SingletonExample.Example();
        }

        [TestMethod]
        public void Builder()
        {
            BuilderExample.Example();
        }

        [TestMethod]
        public void Prototype()
        {
            PrototypeExample.Test();
        }
    }
}
