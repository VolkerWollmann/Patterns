using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.BehaviourPatterns;
using Patterns.Other;

namespace PatternTests
{
    [TestClass]
    public class OtherPatternsTests
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
