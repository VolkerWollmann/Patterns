using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void UnitOfWork()
        {
            UnitOfWorkExample.UnitOfWork();
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
