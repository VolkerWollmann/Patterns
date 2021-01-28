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
    public class StrucutualPatterns
    {
        [TestMethod]
        public void Adapter()
        {
            AdapterExample.Adapter();
        }

        [TestMethod]
        public void Composite()
        {
            CompositeExample.Composite();
        }
    }
}
