using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.CreationalPatterns;

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
    }
}
