using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.BehaviourPatterns;

namespace PatternTests
{
    [TestClass]
    public class BehaviourPatternsTests
    {
        [TestMethod]
        public void Visitor()
        {
            // Visitor
            Visitor visitor = new Visitor();
            visitor.Main();
        }

        [TestMethod]
        public void Strategy()
        {
            StrategyExample.Strategy();
        }

        [TestMethod]
        public void Command()
        {
            CommandExample.Command();
        }

        [TestMethod]
        public void Decorator()
        {
            DecoratorExample.Decorator();
        }

        [TestMethod]
        public void Observer()
        {
            ObserverExample.Observer();
        }

        [TestMethod]
        public void State()
        {
            StateExample.Test();
        }
    }
}
