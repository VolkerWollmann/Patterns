using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.VisitorExample;
using BehaviourPatterns.Strategy;
using BehaviourPatterns.Command;
using StrucutralPatterns.Adapter;
using StrucutralPatterns.Composite;
using BehaviourPatterns.Decorator;
using CreationalPatterns.AbstractFactory;
using CreationalPatterns.FactoryMethod;
using Other.DataTransferObject;
using Other.UnitOfWork;
using Other.BusinessDelegate;

namespace PatternTests
{
    [TestClass]
    public class BehaviourPatterns
    {
        [TestMethod]
        public void Visitor()
        {
            // Visitor
            Vistor visitor = new Vistor();
            visitor.Main();
        }

        [TestMethod]
        public void Visitor_Simple()
        {
            VisitorExample.SimpleVisitor();
        }

        [TestMethod]
        public void Visitor_Transforming()
        {
            VisitorExample.TransformingVisitor();
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
    }
}
