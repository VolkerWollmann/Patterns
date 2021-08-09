using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.VisitorExample;
using BehaviourPatterns.Strategy;
using BehaviourPatterns.Command;
using BehaviourPatterns.Decorator;
using BehaviourPatterns.Observer;
using BehaviourPatterns.State;

namespace PatternTests
{
    [TestClass]
    public class BehaviourPatternsTests
    {
        [TestMethod]
        public void Visitor()
        {
            // Visitor
            Vistor visitor = new Vistor();
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
            BehaviourPatterns.State.StateExample.Test();
        }
    }
}
