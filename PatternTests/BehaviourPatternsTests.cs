using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.BehaviourPatterns;
using Patterns.Structural;

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
        public void Observer()
        {
            ObserverExample.Observer();
        }

        [TestMethod]
        public void State()
        {
            StateExample.Test();
        }

        [TestMethod]
        public void ChainOfResponsibility()
        {
            ChainOfResponsibilityExample.Example();
        }

        [TestMethod]
        public void Iterator()
        {
            IteratorExample.Iterator();
        }

        [TestMethod]
        public void Interpreter()
        {
            InterpreterExample.Example();
        }

        [TestMethod]
        public void Mediator()
        {
            MediatorExample.Example();
        }

        [TestMethod]
        public void Memento()
        {
            MementoExample.Example();
        }
    }
}
