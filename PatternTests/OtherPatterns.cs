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
using Other.DependencyInjection;

namespace PatternTests
{
    [TestClass]
    public class OtherPatterns
    {
        [TestMethod]
        public void DataTransferObject()
        {
            DataTransferObjectExample dataTransferObjectExample = new DataTransferObjectExample();
            dataTransferObjectExample.StartDemo();
        }

        [TestMethod]
        public void UnitOFWork()
        {
            UnitOFWorkExample.UnitOfWork();
        }

        [TestMethod]
        public void BusinessDelegate()
        {
            BusinessDelegateExample.BusinessDelegate();
        }

        [TestMethod]
        public void DependencyInjection()
        {
            DependencyInjectionExample.DependencyInjection();
        }
    }
}
