using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BehaviourPatterns.Visitor;
using BehaviourPatterns.VisitorExample;
using BehaviourPatterns.Strategy;
using BehaviourPatterns.Command;
using StructuralPatterns.Adapter;
using StructuralPatterns.Composite;
using BehaviourPatterns.Decorator;
using CreationalPatterns.AbstractFactory;
using CreationalPatterns.FactoryMethod;
using Other.DataTransferObject;
using Other.UnitOfWork;
using Other.BusinessDelegate;

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
            FactoryMethodExample.FactoryMethod();
        }
    }
}
