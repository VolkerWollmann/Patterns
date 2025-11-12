
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.BehaviourPatterns;
using Xunit;

namespace PatternTests
{
    [TestClass]
    public class BehaviourPatternsTests
    {
        [Fact]
        public void Visitor()
        {
            // Visitor
            Visitor visitor = new Visitor();
            visitor.Main();
        }

        [Fact]
        public void Strategy()
        {
            StrategyExample.Strategy();
        }

        [Fact]
        public void Command()
        {
            CommandExample.Command();
        }

        [Fact]
        public void Observer()
        {
            ObserverExample.Observer();
        }

        [Fact]
        public void State()
        {
            StateExample.Test();
        }

        [Fact]
        public void ChainOfResponsibility()
        {
            ChainOfResponsibilityExample.Example();
        }

        [Fact]
        public void Iterator()
        {
            IteratorExample.Iterator();
        }

        [Fact]
        public void Interpreter()
        {
            InterpreterExample.Example();
        }

        [Fact]
        public void Mediator()
        {
            MediatorExample.Example();
        }

        [Fact]
        public void Memento()
        {
            MementoExample.Example();
        }

        [Fact]
        public void TemplateMethod()
        {
            TemplateMethodExample.Example();
        }
    }
}
